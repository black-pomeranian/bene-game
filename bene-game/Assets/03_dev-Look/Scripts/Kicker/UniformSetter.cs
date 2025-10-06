using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniformSetter : MonoBehaviour
{
    // 公開変数としてマテリアルのリストを用意
    // インスペクターから設定できるようにします
    public List<Material> uniformMaterials = new List<Material>();
    public List<Material> pantsMaterials = new List<Material>();

    // 子要素として取得するゲームオブジェクト
    // インスペクターで設定するか、Start()で子要素から探します
    private GameObject jersey;
    private GameObject soccerPlayer; // 一般的なメッシュ部分（肌など）を想定

    // どのマテリアルを適用するか指定するインデックス
    // 0以上の整数を設定し、uniformMaterialsの要素数を超えないようにします
    [Range(0, 2)] // インスペクターでのスライダーの範囲を指定（適宜変更してください）
    public int selectedUniformIndex = 0;

    // Start()の代わりにAwake()で子要素の取得を試みるのがより一般的です
    void Awake()
    {
        // 子要素の取得（名前で探すのが一般的ですが、ここではTransformの子として設定されている前提）
        // 実際には子の名前が正確である必要があります
        if (jersey == null)
        {
            // Transform.Findは直接の子要素のみを検索します
            Transform jerseyTransform = transform.Find("jersey");
            if (jerseyTransform != null)
            {
                jersey = jerseyTransform.gameObject;
            }
        }

        if (soccerPlayer == null)
        {
            Transform playerTransform = transform.Find("soccerplayer");
            if (playerTransform != null)
            {
                soccerPlayer = playerTransform.gameObject;
            }
        }
    }

    // マテリアルの更新処理をカプセル化
    public void SetUniformMaterial(int index)
    {
        // インデックスが有効な範囲内か確認
        if (index >= 0 && index < uniformMaterials.Count)
        {
            Material uniformMaterial = uniformMaterials[index];

            // jerseyゲームオブジェクトのマテリアルを更新
            if (jersey != null)
            {
                Renderer jerseyRenderer = jersey.GetComponent<Renderer>();
                if (jerseyRenderer != null)
                {
                    // 共有マテリアルではなく、インスタンスマテリアルを更新する場合は .material を使います
                    // 全てのインスタンスに影響を与えたい場合は .sharedMaterial を使います
                    jerseyRenderer.material = uniformMaterial;
                }
            }

            // soccerplayerゲームオブジェクトのマテリアルを更新（もし適用したいメッシュがあれば）
            // この例ではジャージのみを更新し、プレイヤーの肌などを変更しない場合は以下のブロックは不要です

            Material pantsMaterial = pantsMaterials[index];

            if (soccerPlayer != null)
            {
                Renderer playerRenderer = soccerPlayer.GetComponent<Renderer>();
                if (playerRenderer != null)
                {
                    // soccerPlayerの他のマテリアルを更新したい場合は、マテリアル配列のインデックスを指定する必要があります
                    // 例：playerRenderer.materials[1] = targetMaterial;
                    playerRenderer.material = pantsMaterial;
                }
            }

            // 更新されたインデックスを保存
            selectedUniformIndex = index;

        }
        else
        {
            Debug.LogError($"指定されたインデックス {index} はマテリアルのリストの範囲外です。 リストのサイズ: {uniformMaterials.Count}");
        }
    }

    void Start()
    {
        // ゲーム開始時にインスペクターで指定されたインデックスのマテリアルを適用
        SetUniformMaterial(selectedUniformIndex);
    }

    // 開発中にインスペクターで selectedUniformIndex が変更された時に自動で更新したい場合は Update() に以下の処理を追加
    private int _lastIndex = -1;
    void Update()
    {
        if (_lastIndex != selectedUniformIndex)
        {
            SetUniformMaterial(selectedUniformIndex);
            _lastIndex = selectedUniformIndex;
        }
    }
}