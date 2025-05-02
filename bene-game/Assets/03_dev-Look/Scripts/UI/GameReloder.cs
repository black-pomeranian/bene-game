using UnityEngine;
using UnityEngine.SceneManagement;

public class GameReloader : MonoBehaviour
{
    public void ReloadScene()
    {
        // 現在のアクティブなシーンの名前を取得
        string currentSceneName = SceneManager.GetActiveScene().name;

        // そのシーンを再度ロード
        SceneManager.LoadScene(currentSceneName);
    }
}