using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Referee : MonoBehaviour
{
    // 状態を表す列挙型
    public enum RefereeState
    {
        INIT,
        MAKE_CHARACTER,
        STANDBY,
        JUDGE,
        SCORE
    }

    private enum PlayerRule
    {
        KICKER,
        KEEPER
    }

    // シリアライザブル
    [Header("ゲーム用パラメーター")]
    [SerializeField] private float goalJudgeTime = 5.0f;
    [SerializeField, Tooltip("通常行うべきゲームの数。ただし、勝利が確定、決着がつかない場合ゲーム数は変更される")]
    private int gameTurn = 5;

    [Header("キッカー用変数")]
    [SerializeField] private Kicker kickerObj;
    [SerializeField] private KickerCpu kickerCpuObj;
    [SerializeField] private Transform kickerInitTransform;
    [SerializeField] private Transform kickerCameraTransform;

    [Header("キーパー用変数")]
    [SerializeField] private Keeper keeperObj;
    [SerializeField] private KeeperCpu keeperCpuObj;
    [SerializeField] private Transform keeperInitTransform;
    [SerializeField] private Transform keeperCameraTransform;

    [Header("UIマネージャー")]
    [SerializeField] private UIManager uiManager;

    [Header("Player用変数")]
    [SerializeField] private ScoreUIManager playerScoreUIManager;

    [Header("CPU用変数")]
    [SerializeField] private ScoreUIManager cpuScoreUIManager;

    [Header("ボール用変数")]
    [SerializeField] private Ball ballObj;
    [SerializeField] private Transform ballInitTransform;

    [Header("SEマネージャー")]
    [SerializeField] private SePlayer sePlayer;

    [Header("ゲーム設定用変数")]
    [SerializeField] private float kickStartWaitTime = 1.0f;


    // 現在の状態
    private RefereeState currentState = RefereeState.INIT;

    // コンポーネント参照
    private Camera mainCamera;
    private CameraShaker cameraShaker;
    private Coroutine shakeCoroutine;

    private Kicker kicker;
    private Keeper keeper;
    private Ball ball;

    // タイマー変数
    private float currentTimer = 0.0f;

    // スコア管理
    private int playerScore = 0;
    private int cpuScore = 0;

    // ゲーム数管理(ターン数ではなく攻守交替の回数と等価)
    private int currentGameCount = 0;

    // プレイヤーの役割保持
    private PlayerRule playerRule = PlayerRule.KICKER;

    // フラグ
    private bool isGoal = false;
    private bool isGameStarted = false;

    // スタート時の初期化
    void Start()
    {
        mainCamera = Camera.main;
        cameraShaker = mainCamera.GetComponent<CameraShaker>();

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
            case RefereeState.MAKE_CHARACTER:
                UpdateMakeCharacterState();
                break;
            case RefereeState.STANDBY:
                UpdateStandbyState();
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
                ResetParameters();
                break;
            case RefereeState.MAKE_CHARACTER:
                currentTimer = 0.0f;
                // 現在の役割と逆の役割にトグルしてから
                if (playerRule == PlayerRule.KICKER)
                {
                    // 次のターンPlayerはKeeper
                    playerRule = PlayerRule.KEEPER;
                    kicker = Instantiate(kickerCpuObj, kickerInitTransform.position, kickerInitTransform.rotation);
                    keeper = Instantiate(keeperObj, keeperInitTransform.position, keeperInitTransform.rotation);
                }
                else
                {
                    // 次のターンPlayerはKicker
                    playerRule = PlayerRule.KICKER;
                    kicker = Instantiate(kickerObj, kickerInitTransform.position, kickerInitTransform.rotation);
                    keeper = Instantiate(keeperCpuObj, keeperInitTransform.position, keeperInitTransform.rotation);
                }

                kicker.OnKicked += HandleKicked;

                ball = Instantiate(ballObj, ballInitTransform.position, ballInitTransform.rotation);
                ball.Initialize();

                // オブジェクト初期化
                kicker.ChangeState(Kicker.KickerState.STANDBY);
                keeper.ChangeState(Keeper.KeeperState.STANDBY);
                break;
            case RefereeState.STANDBY:
                // キッカーシュート待ち
                sePlayer.PlayWhistleSE();
                kicker.ChangeState(Kicker.KickerState.WAIT);
                keeper.ChangeState(Keeper.KeeperState.WAIT);
                break;
            case RefereeState.JUDGE:
                // ボール挙動時間計測開始
                currentTimer = 0.0f;
                isGoal = false;
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
        if (isGameStarted)
        {
            ChangeState(RefereeState.MAKE_CHARACTER);
        }
    }

    private void UpdateMakeCharacterState()
    {
        // カメラ移動などの処理
        if (playerRule == PlayerRule.KICKER)
        {
            MoveCameraToPlayer(kickerCameraTransform);
        }
        else
        {
            MoveCameraToPlayer(keeperCameraTransform);
            MoveCameraToPlayer(keeperCameraTransform);
        }

        // カメラ移動が完了したら次へ
        if (kickStartWaitTime < currentTimer)
        {
            ChangeState(RefereeState.STANDBY);
        }

        currentTimer += Time.deltaTime;
    }

    private void UpdateStandbyState()
    {
        if (kicker != null && kicker.GetCurrentState() == Kicker.KickerState.WATCH)
        {
            ChangeState(RefereeState.JUDGE);
        }
    }

    private void UpdateJudgeState()
    {
        // ボール挙動時間計測中
        currentTimer += Time.deltaTime;

        // ボール挙動時間超過またはゴールしたら
        // 以降ゴールかどうかはisGoalで判断。
        if (currentTimer >= goalJudgeTime)
        {
            ChangeState(RefereeState.SCORE);
        }

        if (isGoal)
        {
            // GOAL演出表示
            // Debug.Log("GOAL!!!");
        }
    }

    private void UpdateScoreState()
    {
        // [TODO]スコア表示中

        // キーパーとキッカーに結果を通知
        if (isGoal)
        {
            // ゴールの場合
            if (kicker != null)
                kicker.ChangeState(Kicker.KickerState.GOAL);

            if (keeper != null)
                keeper.ChangeState(Keeper.KeeperState.NOTGUARDED);
        }
        else
        {
            // ノーゴールの場合
            if (kicker != null)
                kicker.ChangeState(Kicker.KickerState.NOGOAL);

            if (keeper != null)
                keeper.ChangeState(Keeper.KeeperState.GUARDED);
        }

        // 次のラウンドへ
        if (ShouldContinueGame())
        {
            Debug.Log("PLAYER vs CPU: " + playerScore + " | " + cpuScore);
            ChangeState(RefereeState.MAKE_CHARACTER);
        }
        else
        {
            if (playerScore > cpuScore)
            {
                uiManager.SetResult("WIN", playerScore, cpuScore);
            }
            else
            {
                uiManager.SetResult("LOSE", playerScore, cpuScore);
            }
            sePlayer.PlayGameEndSE();
            GameEnd();
            ChangeState(RefereeState.INIT);
        }
    }

    // 状態から抜けるときの処理
    private void ExitState(RefereeState state)
    {
        switch (state)
        {
            case RefereeState.INIT:
                break;
            case RefereeState.MAKE_CHARACTER:
                // キッカーとキーパーを配置、カメラ移動
                break;
            case RefereeState.STANDBY:
                currentGameCount++;
                break;
            case RefereeState.JUDGE:
                StopCoroutine(shakeCoroutine);
                break;
            case RefereeState.SCORE:
                ResetGame();
                break;
        }
    }

    public void GameStart()
    {
        isGameStarted = true;
    }

    public void GameEnd()
    {
        isGameStarted = false;
        uiManager.DisableGameUI();
        uiManager.EnableEndUI();
    }


    private void ResetParameters()
    {
        playerScore = 0;
        cpuScore = 0;
        playerScoreUIManager.ResetScoreBord();
        cpuScoreUIManager.ResetScoreBord();

        currentGameCount = 0;

        playerRule = PlayerRule.KEEPER;

        currentTimer = 0.0f;
        isGoal = false;
    }

    private void HandleKicked(object sender, KickEventArgs e)
    {
        sePlayer.PlayKickSE();

        if(keeper != null && playerRule == PlayerRule.KICKER)
        {
            // CPU用にキック情報をセットする
            SwipeDirection direction = (SwipeDirection)(Random.Range(0, (int)SwipeDirection.None));
            float arrivalTime = 0.5f;
            keeper.SetDiveInfoFromKick(direction, arrivalTime);
        }
    }

    private void MoveCameraToPlayer(Transform targetTransform)
    {
        if (mainCamera == null || targetTransform == null)
        {
            Debug.LogWarning("mainCamera または targetTransform が null です。");
            return;
        }

        mainCamera.transform.position = targetTransform.position;
        mainCamera.transform.rotation = targetTransform.rotation;
    }

    // ゴールしたかチェック
    public void NotifyGoal()
    {
        isGoal = true;

        shakeCoroutine = StartCoroutine(cameraShaker.Shake(5.0f, 0.5f, 13f));
        sePlayer.PlayApploudSE();

    }

    // スコア更新
    private void UpdateScores()
    {
        // currentGameCountは1から開始。SetScoreは0から開始する。
        int currentGameTurn = (int)((currentGameCount - 1) / 2);
        if (isGoal)
        {
            if(playerRule == PlayerRule.KICKER)
            {
                playerScore++;
                playerScoreUIManager.SetScore_Goal(currentGameTurn);
            }
            else
            {
                cpuScore++;
                cpuScoreUIManager.SetScore_Goal(currentGameTurn);
            }
        }
        else
        {
            if (playerRule == PlayerRule.KICKER)
            {
                playerScoreUIManager.SetScore_Miss(currentGameTurn);
            }
            else
            {
                cpuScoreUIManager.SetScore_Miss(currentGameTurn);
            }
        }
    }

    // ゲームを続けるかチェック
    private bool ShouldContinueGame()
    {
        int totalTurnsPerTeam = gameTurn;
        int totalTurnsPerGame = totalTurnsPerTeam * 2;
        int remainingTurns = totalTurnsPerGame - currentGameCount;

        // 勝敗が確定しているかチェック（残り回数で逆転できない）
        int playerRemainingShots = (totalTurnsPerTeam) - (currentGameCount + 1) / 2;
        int cpuRemainingShots = (totalTurnsPerTeam) - (currentGameCount / 2);

        if (playerScore == cpuScore)
        {
            return true;
        }

        if (playerScore > cpuScore + cpuRemainingShots)
        {
            return false; // プレイヤーの勝ち確定
        }

        if (cpuScore > playerScore + playerRemainingShots)
        {
            return false; // CPUの勝ち確定
        }

        // 規定回数に達していて、かつ同点でなければ終了
        if ((currentGameCount >= totalTurnsPerGame) && (playerScore != cpuScore) && (currentGameCount%2 == 0))
        {
            return false; // 決着がついた
        }

        return true; // ゲーム続行
    }

    private void ResetGame()
    {
        isGameStarted = false;
        Destroy(kicker.gameObject);
        Destroy(keeper.gameObject);
        Destroy(ball.gameObject);
    }
}