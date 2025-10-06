using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq; // List操作のためにLinqを追加

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

    // selectedIndexが変化したことを検出するための前回の選択インデックス
    private int previousSelectedIndex = 1;

    // GetSelectedIndexで選ばれなかった、CPU側が選択すると想定されるインデックス
    private int cpuIndex = -1;

    // 元のスケールを保持するためのリスト
    private List<Vector3> originalScales = new List<Vector3>();

    // Canvasコンポーネネントをキャッシュするためのリスト
    private List<Canvas> targetCanvases = new List<Canvas>(3);

    // スクリプトの初期化
    void Start()
    {
        if (targetImages.Count != 3)
        {
            Debug.LogError("targetImagesリストには、左、中央、右に対応する3つのImageを設定してください。");
            enabled = false;
            return;
        }

        // 全てのImageの元のローカルスケールを保存し、Canvasコンポーネネントを取得
        foreach (var img in targetImages)
        {
            originalScales.Add(img.transform.localScale);

            // Imageと同じGameObjectに付いているCanvasコンポーネントを取得
            Canvas canvas = img.GetComponent<Canvas>();
            if (canvas == null)
            {
                Debug.LogError($"Image '{img.name}' に Canvas コンポーネントが見つかりませんでした。Sorting Orderの制御にはCanvasが必要です。");
                enabled = false;
                return;
            }

            // Sorting Orderを個別に制御するため、Override Sortingを有効にする
            canvas.overrideSorting = true;
            targetCanvases.Add(canvas);
        }

        // 初期の状態 (中央が選択されている状態) を反映させる
        UpdateUIScales();

        // 初期のselectedIndexに基づいて、CPUインデックスも初期化する
        UpdateCPUIndex();
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

        previousSelectedIndex = selectedIndex; // 現在の選択インデックスを保存

        // 2. 累積値に基づいてインデックスを分類
        UpdateClassification();

        // 3. selectedIndexが更新されたかチェックし、CPUインデックスを更新する
        if (selectedIndex != previousSelectedIndex)
        {
            UpdateCPUIndex();
        }

        // 4. UIのサイズとSorting Orderを更新
        UpdateUIScales();

        // 5. 累積値が閾値を超えたらリセット
        CheckAndResetAccumulation();

        // デバッグログ (GetCPUIndexの動作確認用)
        // Debug.Log($"Selected: {selectedIndex}, CPU: {cpuIndex}");
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

    /// <summary>
    /// selectedIndex以外のインデックスからランダムに1つ選び、cpuIndexを更新します。
    /// selectedIndexが更新されたときにのみ呼び出されます。
    /// </summary>
    void UpdateCPUIndex()
    {
        // 0, 1, 2 のインデックスリスト
        List<int> allIndices = new List<int> { 0, 1, 2 };

        // selectedIndex を除いた残りのインデックスのリスト
        List<int> availableIndices = allIndices.Where(i => i != selectedIndex).ToList();

        // 残りのインデックスからランダムに1つを選択
        if (availableIndices.Count > 0)
        {
            int randomIndex = Random.Range(0, availableIndices.Count); // 0 or 1
            cpuIndex = availableIndices[randomIndex];
        }
        else
        {
            // 発生しないはずだが、念のため
            cpuIndex = -1;
        }
    }

    // UIのスケールとSorting Orderを更新するメソッド
    void UpdateUIScales()
    {
        for (int i = 0; i < targetImages.Count; i++)
        {
            Vector3 targetScale;
            int targetSortingOrder;

            if (i == selectedIndex)
            {
                // 現在選択されている要素は拡大し、Sorting Orderを3にする
                targetScale = originalScales[i] * scaleFactor;
                targetSortingOrder = 3; // 選択されたImageを前面に
            }
            else
            {
                // それ以外の要素は元のサイズに戻し、Sorting Orderを2にする
                targetScale = originalScales[i];
                targetSortingOrder = 2; // 非選択のImageを背面に
            }

            // === 1. スケールの変更 (既存の処理) ===
            // Lerp（線形補間）を使ってスムーズにサイズを変更
            targetImages[i].transform.localScale = Vector3.Lerp(
                targetImages[i].transform.localScale,
                targetScale,
                Time.deltaTime * transitionSpeed
            );

            // === 2. Canvas Sorting Order の変更 (追加した処理) ===
            if (targetCanvases[i] != null)
            {
                // Sorting Orderを設定
                targetCanvases[i].sortingOrder = targetSortingOrder;
            }
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

    /// <summary>
    /// GetSelectedIndexで選ばれなかったインデックスの中から、ランダムで選ばれたインデックスを取得します。
    /// この値はselectedIndexが更新されるたびにランダムに再選択されます。
    /// </summary>
    /// <returns>GetSelectedIndexと異なるインデックス (0, 1, または 2) がランダムで設定されます。</returns>
    public int GetCPUIndex()
    {
        return cpuIndex;
    }
}
