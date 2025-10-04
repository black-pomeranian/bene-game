using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keeper : MonoBehaviour
{
    // 状態を表す列挙型
    public enum KeeperState
    {
        STANDBY,
        WAIT,
        AIM,
        GUARD,
        WATCH,
        GUARDED,
        NOTGUARDED
    }

    [System.Serializable]
    public class SwipeMoveOffset
    {
        public SwipeDirection direction;
        public Vector3 offset;
    }

    // シリアライザブル
    [SerializeField] private float diveAnimExitTime = 2.0f;
    [SerializeField] private float diveDistWeight = 5.0f;
    [SerializeField] private AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private List<SwipeMoveOffset> swipeMoveOffsets = new List<SwipeMoveOffset>();
    [SerializeField] protected bool isSideRevert = true;


    // 現在の状態
    private KeeperState currentState = KeeperState.STANDBY;

    // コンポーネント参照
     private KeepAnimController keepAnimController; 
    /* private Referee referee; */

    // 初期位置
    private Vector3 initPosition;
    private Vector3 initRotation;

    // KEEP状態用変数
    public SwipeDirection diveDirection { get; set; }
    public bool isKeeperTouched { get; private set; }
    private Vector2 swipeStartPosition;
    private Vector2 swipeEndPosition;
    private Vector3 diveStartPosition;
    private Vector3 targetPosition;
    private Coroutine guardCoroutine;

    private float elapsedTime;

    // Start is called before the first frame update
    void Start()
    {
        initPosition = this.transform.position;
        initRotation = this.transform.eulerAngles;
        keepAnimController = FindObjectOfType<KeepAnimController>();

        // 初期状態を設定
        ChangeState(KeeperState.STANDBY);
    }

    void Update()
    {
        // 現在の状態に応じた処理を実行
        switch (currentState)
        {
            case KeeperState.STANDBY:
                UpdateStandby();
                break;
            case KeeperState.WAIT:
                UpdateWait();
                break;
            case KeeperState.AIM:
                UpdateAim();
                break;
            case KeeperState.GUARD:
                UpdateGuard();
                break;
            case KeeperState.WATCH:
                UpdateWatch();
                break;
            case KeeperState.GUARDED:
                UpdateGuarded();
                break;
            case KeeperState.NOTGUARDED:
                UpdateNotGuarded();
                break;
        }
    }

    // 状態を変更するメソッド
    public void ChangeState(KeeperState newState)
    {
        // 現在の状態から抜ける
        ExitState(currentState);

        // 状態を更新
        currentState = newState;

        // 新しい状態に入る
        EnterState(currentState);
    }
    
    // 状態に入るときの処理
    protected virtual void EnterState(KeeperState state)
    {
        switch (state)
        {
            case KeeperState.STANDBY:
                break;
            case KeeperState.WAIT:
                // パラメータの位置を初期化
                ResetParameters();
                break;
            case KeeperState.AIM:
                swipeStartPosition = Input.mousePosition;
                break;
            case KeeperState.GUARD:
                // コルーチン開始
                diveStartPosition = this.transform.position;
                if (guardCoroutine != null) StopCoroutine(guardCoroutine);
                guardCoroutine = StartCoroutine(GuardRoutine());

                // 防御アニメーション再生
                Dive();
                break;
            case KeeperState.WATCH:
                // 結果待ち
                break;
            case KeeperState.GUARDED:
                // 成功アニメーション再生
                /*PlaySuccessAnimation();*/
                break;
            case KeeperState.NOTGUARDED:
                // 失敗アニメーション再生
                /*PlayFailAnimation();*/
                break;
        }
    }

    // STANDBY状態の更新
    private void UpdateStandby()
    {
        // Refereeから指示があるまで待機
    }

    // WAIT状態の更新
    protected virtual void UpdateWait()
    {
        // パラメータの位置初期化を行う
        // タッチ入力を待つ
        if (Input.GetMouseButtonDown(0))
        {
            ChangeState(KeeperState.AIM);
        }
    }

    // AIM状態の更新
    protected virtual void UpdateAim()
    {
        // スワイプ終了（指/マウスを離した）
        if (Input.GetMouseButtonUp(0))
        {
            // スワイプの方向と強さを計算
            swipeEndPosition = Input.mousePosition;
            diveDirection = SwipeUtility.GetSwipeDirection(swipeStartPosition, swipeEndPosition);

            if(isSideRevert)
            {
                diveDirection = RevertDirection(diveDirection);
            }

            ChangeState(KeeperState.GUARD);
        }
    }

    // GUARD状態の更新
    private void UpdateGuard()
    {
        // ガードアニメーションが終了したら状態変更
        if (elapsedTime > diveAnimExitTime)
        {
            ChangeState(KeeperState.WATCH);
        }
        
        elapsedTime += Time.deltaTime;
        
    }

    // WATCH状態の更新
    private void UpdateWatch()
    {
        // 結果待ち状態
    }

    // GUARDED状態の更新
    private void UpdateGuarded()
    {

    }

    // NOTGUARDED状態の更新
    private void UpdateNotGuarded()
    {

    }

    // 状態を出るときの処理
    public void ExitState(KeeperState newState)
    {
        // 状態遷移時の処理
        switch (newState)
        {
            case KeeperState.STANDBY:
                break;
            case KeeperState.WAIT:
                break;
            case KeeperState.AIM:
                 break;
            case KeeperState.GUARD:
                DiveExit();
                break;
            case KeeperState.WATCH:
                break;
            case KeeperState.GUARDED:
                break;
            case KeeperState.NOTGUARDED:
                break;
        }
    }

    // パラメータをリセット
    private void ResetParameters()
    {
        /* 位置の初期化 */
        this.transform.position = initPosition;
        this.transform.eulerAngles = initRotation;

        isKeeperTouched = false;

        /* クラス固有パラメーターの初期化 */
        swipeStartPosition = Vector2.zero;
        swipeEndPosition = Vector2.zero;
        diveDirection = SwipeDirection.None;
        /*keepAnimController.StopKeep();*/

        elapsedTime = 0.0f;
    }

    // 方向を逆にする
    public SwipeDirection RevertDirection(SwipeDirection direction)
    {
        if (diveDirection == SwipeDirection.Left)
        {
            return SwipeDirection.Right;
        }
        else if (diveDirection == SwipeDirection.Right)
        {
            return SwipeDirection.Left;
        }
        else if (diveDirection == SwipeDirection.UpperLeft)
        {
            return SwipeDirection.UpperRight;
        }
        else if (diveDirection == SwipeDirection.UpperRight)
        {
            return SwipeDirection.UpperLeft;
        }
        else
        {
            return direction;
        }
    }

    // ダイブ
    private void Dive()
    {
        keepAnimController.PlayDiveAnim(diveDirection);
    }

    // ダイブExit
    private void DiveExit()
    {

        keepAnimController.PlayDiveExitAnim();
    }

    private IEnumerator GuardRoutine()
    {
        // 目標位置を先に計算しておく
        targetPosition = CalculateTargetPosition(diveDirection);

        // 位置がほぼ一致するまでループ
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            // 毎フレーム少しずつ移動
            MoveTowardsTarget(targetPosition);

            yield return null; // 次フレームまで待つ
        }

        if (diveDirection == SwipeDirection.Up || diveDirection == SwipeDirection.UpperLeft || diveDirection == SwipeDirection.UpperRight)
        {
            Vector3 pos = transform.position;
            pos.y -= targetPosition.y;
            transform.position = pos;
        }

        // ループを抜けた＝目的地に到達した
        guardCoroutine = null;
    }
    private void MoveTowardsTarget(Vector3 targetPosition)
    {
        // 正規化した経過時間（0～1）
        float t = elapsedTime / diveAnimExitTime;
        // easeCurveでイージング
        float easedT = easeCurve.Evaluate(t);

        // ターゲットに向かって移動
        transform.position = Vector3.Lerp(
            diveStartPosition,
            targetPosition,
            easedT
        );
    }

    private Vector3 CalculateTargetPosition(SwipeDirection direction)
    {
        // 今の位置
        Vector3 currentPosition = transform.position;

        // 対応するオフセットを探す
        foreach (var swipeOffset in swipeMoveOffsets)
        {
            if (swipeOffset.direction == direction)
            {
                return currentPosition + (swipeOffset.offset * diveDistWeight);
            }
        }

        // なかったら今の位置のまま
        return currentPosition;
    }

    public virtual void SetDiveInfoFromKick(SwipeDirection direction, float arrivalTime)
    {
        // CPU用メソッド
    }

    // 現在の状態を取得するメソッド
    public KeeperState GetCurrentState()
    {
        return currentState;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            isKeeperTouched = true;
        }
    }
}