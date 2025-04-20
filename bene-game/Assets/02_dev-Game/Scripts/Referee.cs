using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Referee : MonoBehaviour
{
    // 状態を表す列挙型
    public enum RefereeState
    {
        INIT,
        CHOOSE_KICKER,
        STANDBY,
        WATCH,
        JUDGE,
        SCORE
    }

    // シリアライザブル
    [SerializeField] private float countdownTime = 3.0f;
    [SerializeField] private float goalJudgeTime = 5.0f;

    // 現在の状態
    private RefereeState currentState = RefereeState.INIT;

    // コンポーネント参照
    private Kicker kicker;
    private Keeper keeper;
    private Ball ball;
    private Camera mainCamera;

    // タイマー変数
    private float currentTimer = 0.0f;

    // スコア管理
    private int kickerScore = 0;
    private int keeperScore = 0;
    private bool isGoal = false;

    // スタート時の初期化
    void Start()
    {
        kicker = FindObjectOfType<Kicker>();
        keeper = FindObjectOfType<Keeper>();
        ball = FindObjectOfType<Ball>();
        mainCamera = Camera.main;

        // 初期状態を設定
        ChangeState(RefereeState.INIT);
    }

    void Update()
    {
        // 現在の状態に応じた処理を実行
        switch (currentState)
        {
            case RefereeState.INIT:
                UpdateInitState();
                break;
            case RefereeState.CHOOSE_KICKER:
                UpdateChooseKickerState();
                break;
            case RefereeState.STANDBY:
                UpdateStandbyState();
                break;
            case RefereeState.WATCH:
                UpdateWatchState();
                break;
            case RefereeState.JUDGE:
                UpdateJudgeState();
                break;
            case RefereeState.SCORE:
                UpdateScoreState();
                break;
        }
    }

    // 状態を変更するメソッド
    public void ChangeState(RefereeState newState)
    {
        // 現在の状態から抜ける
        ExitState(currentState);

        // 状態を更新
        currentState = newState;

        // 新しい状態に入る
        EnterState(currentState);
    }

    // 状態に入るときの処理
    private void EnterState(RefereeState state)
    {
        switch (state)
        {
            case RefereeState.INIT:
                // オブジェクトを初期化
                break;
            case RefereeState.CHOOSE_KICKER:
                // キッカーを決める
                break;
            case RefereeState.STANDBY:
                // カウントダウン開始
                currentTimer = countdownTime;
                break;
            case RefereeState.WATCH:
                // ボール挙動時間計測開始
                currentTimer = 0.0f;
                break;
            case RefereeState.JUDGE:
                // ゴール有無判定計測開始
                currentTimer = 0.0f;
                break;
            case RefereeState.SCORE:
                // スコア計算
                UpdateScores();
                break;
        }
    }

    // 状態を更新する処理
    private void UpdateInitState()
    {
        // プレイヤーのスタート操作やシーンの読み込みが完了したら次へ
        ChangeState(RefereeState.CHOOSE_KICKER);
    }

    private void UpdateChooseKickerState()
    {
        // カメラ移動などの処理
        SwitchCameraToKicker();

        // カメラ移動が完了したら次へ
        ChangeState(RefereeState.STANDBY);
    }

    private void UpdateStandbyState()
    {
        // カウントダウン処理
        currentTimer -= Time.deltaTime;

        if (currentTimer <= 0)
        {
            // カウントダウン終了、キッカーとキーパーに通知
            if (kicker != null)
                kicker.SetToWAIT();
            if (keeper != null)
                keeper.SetToWAIT();

            ChangeState(RefereeState.WATCH);
        }
    }

    private void UpdateWatchState()
    {
        // ボール挙動時間計測中

        // キッカーがKICK状態になったら監視開始
        if (kicker != null && kicker.GetCurrentState() == Kicker.KickerState.KICK)
        {
            // ボールの動きを監視
        }

        // ボール挙動時間計測終了条件
        if (IsGoalJudgePossible())
        {
            ChangeState(RefereeState.JUDGE);
        }
    }

    private void UpdateJudgeState()
    {
        // ゴール判定時間計測
        currentTimer += Time.deltaTime;

        // ゴールに入った場合
        if (CheckGoal())
        {
            isGoal = true;
            ChangeState(RefereeState.SCORE);
        }

        // 時間切れ
        if (currentTimer >= goalJudgeTime)
        {
            isGoal = false;
            ChangeState(RefereeState.SCORE);
        }
    }

    private void UpdateScoreState()
    {
        // スコア表示中

        // キーパーとキッカーに結果を通知
        if (isGoal)
        {
            // ゴールの場合
            if (kicker != null && kicker.GetCurrentState() != Kicker.KickerState.GOAL)
                kicker.ChangeState(Kicker.KickerState.GOAL);

            if (keeper != null && keeper.GetCurrentState() != Keeper.KeeperState.NOTGUARDED)
                keeper.ChangeState(Keeper.KeeperState.NOTGUARDED);
        }
        else
        {
            // ノーゴールの場合
            if (kicker != null && kicker.GetCurrentState() != Kicker.KickerState.NOGOAL)
                kicker.ChangeState(Kicker.KickerState.NOGOAL);

            if (keeper != null && keeper.GetCurrentState() != Keeper.KeeperState.GUARDED)
                keeper.ChangeState(Keeper.KeeperState.GUARDED);
        }

        // 次のラウンドへ
        if (ShouldContinueGame())
        {
            ChangeState(RefereeState.CHOOSE_KICKER);
        }
        else
        {
            // ゲーム終了処理
        }
    }

    // 状態から抜けるときの処理
    private void ExitState(RefereeState state)
    {
        switch (state)
        {
            case RefereeState.INIT:
                break;
            case RefereeState.CHOOSE_KICKER:
                // キッカーとキーパーを左右に配置、カメラ移動
                break;
            case RefereeState.STANDBY:
                // WAIT状態へ遷移
                break;
            case RefereeState.WATCH:
                break;
            case RefereeState.JUDGE:
                break;
            case RefereeState.SCORE:
                break;
        }
    }

    // ゴール判定が可能かチェック
    private bool IsGoalJudgePossible()
    {
        // ボールが停止したか一定時間経過したかを判定
        return ball != null && (kicker.GetCurrentState() == Kicker.KickerState.WATCH);
    }

    // ゴールしたかチェック
    private bool CheckGoal()
    {
        /*return goalNet != null && goalNet.IsGoal();*/
        return true;
    }

    // スコア更新
    private void UpdateScores()
    {
        if (isGoal)
        {
            kickerScore++;
        }
        else
        {
            keeperScore++;
        }
    }

    // ゲームを続けるかチェック
    private bool ShouldContinueGame()
    {
        // 最大ラウンド数やスコア条件に基づいて判定
        return true; // 仮実装
    }

    // カメラをキッカー側に切り替え
    private void SwitchCameraToKicker()
    {
        if (mainCamera != null)
        {
            // カメラの位置と向きを設定
            // mainCamera.transform.position = ...
            // mainCamera.transform.rotation = ...
        }
    }

    // キッカーを表示中かどうか
    public bool IsShowingKicker()
    {
        return currentState == RefereeState.CHOOSE_KICKER || currentState == RefereeState.STANDBY;
    }

    // スコアへ入ったか
    public bool IsInScoreState()
    {
        return currentState == RefereeState.SCORE;
    }
}