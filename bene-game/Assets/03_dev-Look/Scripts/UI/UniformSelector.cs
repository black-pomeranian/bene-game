using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UniformSelector : MonoBehaviour
{
    // === 外部から設定する変数 ===
    [Header("UI設定")]
    // [0]は左、[1]は中央、[2]は右に対応させる
    public List<Image> targetImages = new List<Image>(3);

    // UIの拡大率
    public float scaleFactor = 1.5f;

    // UIをターゲットサイズへ変更する速さ
    [Tooltip("UIを新しいターゲットサイズに変更する補間速度")]
    public float transitionSpeed = 10f;

    [Header("累積値の設定")]
    // 累積値をリセットするまでの合計移動量の閾値 (例: 100以上になるとリセット)
    [Tooltip("累積移動量の合計がこの値を超えたらリセットします")]
    public float resetThreshold = 100f;

    // 左右の移動量の影響度 (感度)
    [Tooltip("マウス移動量に乗算される係数")]
    public float sensitivity = 1.0f;

    // 中央の判定を優先するための調整係数
    [Tooltip("累積移動量が左右に偏っていない場合に中央のインデックスを選択しやすくするための係数")]
    public float centerBias = 0.5f;

    // === 内部変数 ===
    // 左右それぞれの累積移動量（絶対値の合計）
    private float accumulatedLeft = 0f;
    private float accumulatedRight = 0f;

    // 選択されたパターンのインデックス (0:左, 1:中央, 2:右)
    private int selectedIndex = 1;

    // 元のスケールを保持するためのリスト
    private List<Vector3> originalScales = new List<Vector3>();

    // スクリプトの初期化
    void Start()
    {
        if (targetImages.Count != 3)
        {
            Debug.LogError("targetImagesリストには、左、中央、右に対応する3つのImageを設定してください。");
            enabled = false;
            return;
        }

        // 全てのImageの元のローカルスケールを保存
        foreach (var img in targetImages)
        {
            originalScales.Add(img.transform.localScale);
        }

        // 初期の状態 (中央が選択されている状態) を反映させる
        UpdateUIScales();
    }

    // 毎フレーム実行される更新処理
    void Update()
    {
        // 1. マウスのベクトルを取得し、加速度の大きさを左右に加算
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;

        if (mouseX < 0)
        {
            // 左移動の場合: accumulatedLeftに絶対値を加算
            accumulatedLeft += Mathf.Abs(mouseX);
        }
        else if (mouseX > 0)
        {
            // 右移動の場合: accumulatedRightに絶対値を加算
            accumulatedRight += Mathf.Abs(mouseX);
        }

        // 2. 累積値に基づいてインデックスを分類
        UpdateClassification();

        // 3. UIのサイズを更新
        UpdateUIScales();

        // 4. 累積値が閾値を超えたらリセット
        CheckAndResetAccumulation();

        // デバッグログ
        // Debug.Log($"L: {accumulatedLeft:F2}, R: {accumulatedRight:F2}, Selected: {GetClassificationName(selectedIndex)} ({selectedIndex})");
    }

    // 累積値に基づいてインデックスを分類するメソッド
    void UpdateClassification()
    {
        float totalAccumulation = accumulatedLeft + accumulatedRight;

        if (totalAccumulation == 0f)
        {
            // 移動がまったくない場合（初期状態など）は中央を選択
            selectedIndex = 1;
            return;
        }

        // 各要素の「重み」または「割合」を計算
        float ratioLeft = accumulatedLeft / totalAccumulation;
        float ratioRight = accumulatedRight / totalAccumulation;

        // 中央の割合は、左右の偏りが少ない場合に大きくなるように定義
        // 例：左右が 0.5 の場合、中央の割合は 1.0 になる。 
        // 左右が 1.0/0.0 の場合、中央の割合は 0.0 になる。
        float ratioCenter = 1f - Mathf.Abs(ratioLeft - ratioRight);

        // 中央の重みは、ユーザー設定のcenterBiasで調整可能にする
        float weightedCenter = ratioCenter + centerBias;

        // 最大の割合を持つインデックスを選択
        if (ratioLeft > weightedCenter && ratioLeft > ratioRight)
        {
            // 左の累積が支配的
            selectedIndex = 0; // 左
        }
        else if (ratioRight > weightedCenter && ratioRight > ratioLeft)
        {
            // 右の累積が支配的
            selectedIndex = 2; // 右
        }
        else
        {
            // 左右のバランスが取れているか、または中央のバイアスが大きいため中央が選択される
            selectedIndex = 1; // 中央
        }
    }

    // UIのスケールを更新するメソッド
    void UpdateUIScales()
    {
        for (int i = 0; i < targetImages.Count; i++)
        {
            Vector3 targetScale;

            if (i == selectedIndex)
            {
                // 現在選択されている要素は拡大
                targetScale = originalScales[i] * scaleFactor;
            }
            else
            {
                // それ以外の要素は元のサイズ
                targetScale = originalScales[i];
            }

            // Lerp（線形補間）を使ってスムーズにサイズを変更
            targetImages[i].transform.localScale = Vector3.Lerp(
                targetImages[i].transform.localScale,
                targetScale,
                Time.deltaTime * transitionSpeed
            );
        }
    }

    // 累積値が閾値を超えたらリセットするメソッド
    void CheckAndResetAccumulation()
    {
        if (accumulatedLeft + accumulatedRight > resetThreshold)
        {
            // 累積値を合計でリセットし、新たな累積を始める
            accumulatedLeft = 0f;
            accumulatedRight = 0f;
            // リセットと同時に中央に戻すかどうかは仕様によりますが、ここではそのままの状態を維持します。
            // selectedIndex = 1; // リセット時に中央に戻したい場合はコメントアウトを解除
        }
    }

    // デバッグ用の分類名取得
    string GetClassificationName(int index)
    {
        switch (index)
        {
            case 0: return "左";
            case 1: return "中央";
            case 2: return "右";
            default: return "不明";
        }
    }

    /// <summary>
    /// 現在の累積値に基づいて選択されたインデックスを取得します。
    /// </summary>
    /// <returns>0:左, 1:中央, 2:右</returns>
    public int GetSelectedIndex()
    {
        return selectedIndex;
    }
}