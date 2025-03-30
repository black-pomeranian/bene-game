using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kicker : MonoBehaviour
{
    // 状態を表す列挙型
    public enum KickerState
    {
        STANDBY,
        WAIT,
        AIM,
        KICK,
        WATCH,
        GOAL,
        NOGOAL
    }

    [SerializeField] private float watchTime = 5.0f;

    // 現在の状態
    private KickerState currentState = KickerState.STANDBY;

    // コンポーネント参照
    private KickAnimController kickAnimController;
    private Ball ball;
    /* private Referee referee; */

    // 初期位置
    private Vector3 initPosition;
    private Vector3 initRotation;

    // KICK状態用変数    
    private Vector3 aimVector3;
    private Vector2 swipeStartPosition;
    private Vector2 swipeEndPosition;
    private bool isTouchedBall;

    // WATCH状態用変数
    private float watchCurrentTimer;
    private bool isGoal;

    // 初期化
    void Start()
    {
        initPosition = this.transform.position;
        initRotation = this.transform.eulerAngles;
        watchCurrentTimer = 0.0f;
        kickAnimController = FindObjectOfType<KickAnimController>();
        ball = FindObjectOfType<Ball>();
        /*referee = FindObjectOfType<Referee>();*/

        // 初期状態を設定
        ChangeState(KickerState.WAIT);
    }

    // 更新
    void Update()
    {
        // 現在の状態に応じた処理を実行
        switch (currentState)
        {
            case KickerState.STANDBY:
                UpdateStandbyState();
                break;
            case KickerState.WAIT:
                UpdateWaitState();
                break;
            case KickerState.AIM:
                UpdateAimState();
                break;
            case KickerState.KICK:
                UpdateKickState();
                break;
            case KickerState.WATCH:
                UpdateWatchState();
                break;
            case KickerState.GOAL:
                UpdateGoalState();
                break;
            case KickerState.NOGOAL:
                UpdateNoGoalState();
                break;
        }
    }

    // 状態を変更するメソッド
    public void ChangeState(KickerState newState)
    {
        // 現在の状態から抜ける
        ExitState(currentState);

        // 状態を更新
        currentState = newState;

        // 新しい状態に入る
        EnterState(currentState);
    }

    // 状態に入るときの処理
    private void EnterState(KickerState state)
    {
        switch (state)
        {
            case KickerState.STANDBY:
                // 何もしない
                break;
            case KickerState.WAIT:
                // パラメータの初期化
                this.transform.position = initPosition;
                this.transform.eulerAngles = initRotation;
                ball.Initialize();
                isTouchedBall = false;
                isGoal = false;
                aimVector3 = Vector3.zero;
                kickAnimController.StopKick();
                Debug.Log("Enter Wait state");
                break;
            case KickerState.AIM:
                // スワイプ開始位置を記録
                swipeStartPosition = Input.mousePosition;
                break;
            case KickerState.KICK:
                // キックアニメーション再生
                if (kickAnimController != null)
                    kickAnimController.StartKick();
                break;
            case KickerState.WATCH:
                // 何もしない
                break;
            case KickerState.GOAL:
                // ゴール時のアニメーション再生
                /*if (animator != null)
                    animator.SetTrigger("Goal");*/
                break;
            case KickerState.NOGOAL:
                // 失敗時のアニメーション再生
                /*if (animator != null)
                    animator.SetTrigger("NoGoal");*/
                break;
        }
    }

    // 状態を更新する処理
    private void UpdateStandbyState()
    {
        // レフェリーから指示があったらWAIT状態へ
        /* (referee != null && referee.IsShowingKicker())
        {
            ChangeState(KickerState.WAIT);
        }*/
    }

    private void UpdateWaitState()
    {
        // タッチ/クリック入力検出
        if (Input.GetMouseButtonDown(0))
        {
            ChangeState(KickerState.AIM);
        }
    }

    private void UpdateAimState()
    {
        // スワイプ中
        if (Input.GetMouseButton(0))
        {
            // スワイプの方向と強さを計算
            Vector2 currentPosition = Input.mousePosition;
            Vector2 aimVector2 = swipeStartPosition - currentPosition;

            /* 2次元のタッチ情報のベクトルを3次元へマッピング */
            float forceScale = 0.1f;
            aimVector3 = new Vector3(aimVector2.x * forceScale, Mathf.Abs(aimVector2.y) * forceScale, aimVector2.y * forceScale);

            // デバッグ表示
            Debug.DrawRay(transform.position, aimVector3, Color.red);
        }

        // スワイプ終了（指/マウスを離した）
        if (Input.GetMouseButtonUp(0))
        {
            swipeEndPosition = Input.mousePosition;
            Debug.Log("Aim: " + aimVector3);
            ChangeState(KickerState.KICK);
        }
    }

    private void UpdateKickState()
    {
        // ボールに接触したら次のステートへ遷移する
        if (isTouchedBall)
        {
            ChangeState(KickerState.WATCH);
        }

    }

    private void UpdateWatchState()
    {
        // ボールが入るかどうか待つ
        watchCurrentTimer += Time.deltaTime;

        if (watchCurrentTimer > watchTime)
        {
            ChangeState(KickerState.NOGOAL);
        }

        if (isGoal)
        {
            ChangeState(KickerState.GOAL);
        }
    }

    private void UpdateGoalState()
    {
        // ゴール状態での更新処理
        // アニメーション完了を待つなど

        // [DEBUG]
        ChangeState(KickerState.WAIT);
        
    }

    private void UpdateNoGoalState()
    {
        // ノーゴール状態での更新処理
        // アニメーション完了を待つなど
        // [DEBUG]
        ChangeState(KickerState.WAIT);
    }

    // 状態から抜けるときの処理
    private void ExitState(KickerState state)
    {
        switch (state)
        {
            case KickerState.STANDBY:
                break;
            case KickerState.WAIT:
                break;
            case KickerState.AIM:
                break;
            case KickerState.KICK:
                // ボールに力を加える
                if (ball != null)
                    ball.AddForce(aimVector3);
                break;
            case KickerState.GOAL:
                break;
            case KickerState.NOGOAL:
                break;
        }
    }

    // レフェリーによって状態をWAIT状態にするためのパブリックメソッド
    public void SetToWAIT()
    {
        ChangeState(KickerState.WAIT);
    }

    // 現在の状態を取得するメソッド
    public KickerState GetCurrentState()    
    {
        return currentState;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            isTouchedBall = true;
        }
    }
}