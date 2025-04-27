using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartButtonManager : MonoBehaviour
{

    private Button button;
    [SerializeField] GameManager gameManager;


    void Start()
    {
        // ボタンコンポーネントを取得
        button = GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError("Buttonコンポーネントがアタッチされていません。");
            enabled = false; // スクリプトを無効にする
            return;
        }

        // ボタンのクリックイベントにリスナーを追加
        button.onClick.AddListener(SetStateContents);
    }

    void SetStateContents()
    {

    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
