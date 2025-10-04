using UnityEngine;

public enum GameState
{
    Start,
    Contents,
    End
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState CurrentState { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーン遷移しても破棄されないようにする（必要に応じて）
        }
        else
        {
            Destroy(gameObject);
        }

        // 初期状態をStartに設定
        CurrentState = GameState.Start;
        Debug.Log("ゲーム開始状態: " + CurrentState);
    }

    public void ChangeState(GameState newState)
    {
        if (CurrentState == newState)
        {
            Debug.LogWarning("現在の状態と同じ状態に遷移しようとしました: " + newState);
            return;
        }

        CurrentState = newState;

        // ここで状態に応じた処理を記述できます
        switch (CurrentState)
        {
            case GameState.Start:
                // Start状態の処理
                break;
            case GameState.Contents:
                // Contents状態の処理
                break;
            case GameState.End:
                // End状態の処理
                break;
        }
    }

    // 状態を確認するための簡単なメソッド
    public bool IsStartState()
    {
        return CurrentState == GameState.Start;
    }

    public bool IsContentsState()
    {
        return CurrentState == GameState.Contents;
    }

    public bool IsEndState()
    {
        return CurrentState == GameState.End;
    }
}