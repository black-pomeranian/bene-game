using UnityEngine;

public class GameUtility:MonoBehaviour
{
    [SerializeField] GameObject sound;
    private bool isSoundOn = true;

    private void Start()
    {
        SwipeUtility.CursorDisappear();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (!Cursor.visible)
            {
                isSoundOn = false;
                SwipeUtility.CursorAppear();
            }
            else
            {
                isSoundOn = true;
                SwipeUtility.CursorDisappear();
            }

            sound.SetActive(isSoundOn);
        }

        // ESCキー：フルスクリーン解除または終了
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // --- 方法A: フルスクリーンを解除 ---
            if (Screen.fullScreen)
            {
                Screen.fullScreen = false;
            }
            else
            {
                // --- 方法B: ゲームを終了 ---
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false; // Editorで停止
#else
                Application.Quit(); // ビルド後の実行ファイルで終了
#endif
            }
        }
    }
}
