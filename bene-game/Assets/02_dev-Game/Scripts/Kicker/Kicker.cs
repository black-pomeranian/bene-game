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
    
    // シリアライザブル
    [SerializeField] private float watchTime = 5.0f;
    [SerializeField] public float kickForce = 0.1f;
    [SerializeField] public float maxKickForce = 1000f;
    [SerializeField] public float minKickForce = 300f;
    [SerializeField] public List<Transform> kickTargets = new List<Transform>();

    // 現在の状態
    private KickerState currentState = KickerState.STANDBY;

    // コンポーネント参照
    private KickAnimController kickAnimController;
    public Ball ball {get; private set;}
    /* private Referee referee; */

    // 初期位置
    private Vector3 initPosition;
    private Vector3 initRotation;

    // KICK状態用変数    
    public Vector3 aimVector3 {get; set;}
    private Vector2 swipeStartPosition;
    private Vector2 swipeEndPosition;
    private float swipeStartTime;
    private float swipeEndTime;
    private bool isTouchedBall;

    // WATCH状態用変数
    private float watchCurrentTimer;
    private bool isGoal;

    // 初期化
    void Start()
    {
        initPosition = this.transform.position;
        initRotation = this.transform.eulerAngles;
        kickAnimController = FindObjectOfType<KickAnimController>();
        ball = FindObjectOfType<Ball>();

        // 初期状態を設定
        ChangeState(KickerState.STANDBY);
    }

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
    protected virtual void EnterState(KickerState state)
    {
        switch (state)
        {
            case KickerState.STANDBY:
                break;
            case KickerState.WAIT:
                // パラメータの初期化
                ResetParameters();
                break;
            case KickerState.AIM:
                // スワイプ開始位置を記録
                swipeStartPosition = Input.mousePosition;
                swipeStartTime = Time.time;
                break;
            case KickerState.KICK:
                // キックアニメーション再生
                if (kickAnimController != null)
                    kickAnimController.StartKick();
                break;
            case KickerState.WATCH:
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

    protected virtual void UpdateWaitState()
    {
        // タッチ/クリック入力検出
        if (Input.GetMouseButtonDown(0))
        {
            ChangeState(KickerState.AIM);
        }
    }

    // AIM状態の更新
    protected virtual void UpdateAimState()
    {
        // スワイプ終了（指/マウスを離した）
        if (Input.GetMouseButtonUp(0))
        {
            swipeEndPosition = Input.mousePosition;
            swipeEndTime = Time.time;
            
            // スワイプ時間の計算
            float swipeTime = swipeEndTime - swipeStartTime;
            
            // スワイプ速度の計算（スワイプ距離 / 時間）
            float swipeDistance = Vector2.Distance(swipeStartPosition, swipeEndPosition);
            float swipeSpeed = swipeTime > 0 ? swipeDistance / swipeTime : 0;
            
            // 最も近いターゲットを見つける
            Transform closestTarget = FindClosestTarget(swipeEndPosition);
            
            // ボールからターゲットへの方向ベクトル
            Vector3 directionToTarget = (closestTarget.position - ball.rb.transform.position).normalized;
            
            // スワイプ速度に基づいてキック力を計算（より速いスワイプ = より強いキック）
            float kickPower = Mathf.Clamp(swipeSpeed * kickForce, minKickForce, maxKickForce);
            
            // 最終的なキックベクトルの計算
            aimVector3 = directionToTarget * kickPower;
            
            // 中央下のターゲットの場合は、より平らな軌道に（ローボール）
            
            if (closestTarget == kickTargets[0] || closestTarget == kickTargets[4] || closestTarget == kickTargets[5])
            {
                aimVector3 = new Vector3(aimVector3.x, aimVector3.y*0.1f, aimVector3.z);
            }

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
        
    }

    private void UpdateNoGoalState()
    {
        // ノーゴール状態での更新処理
        // アニメーション完了を待つなど

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

    private void ResetParameters()
    {
        /* 位置の初期化 */
        this.transform.position = initPosition;
        this.transform.eulerAngles = initRotation;

        /* ボールの初期化 */
        ball.Initialize();

        /* クラス固有パラメーターの初期化 */
        isTouchedBall = false;
        isGoal = false;
        watchCurrentTimer = 0.0f;
        aimVector3 = Vector3.zero;
        kickAnimController.StopKick();
    }

    private Transform FindClosestTarget(Vector2 screenPosition)
    {   
        Transform closest = null;
        float closestDistance = float.MaxValue;
        
        foreach (Transform kickTarget in kickTargets)
        {
            // ターゲットのワールド座標をスクリーン座標に変換
            Vector2 targetScreenPos = Camera.main.WorldToScreenPoint(kickTarget.position);
            
            // スクリーン座標での距離を計算
            float distance = Vector2.Distance(screenPosition, targetScreenPos);
            
            // より近いターゲットが見つかれば更新
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = kickTarget;
            }
        }
        
        return closest;
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