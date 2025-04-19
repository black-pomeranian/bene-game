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
        GUARDED,
        NOTGUARDED
    }

    // シリアライザブル
    [SerializeField] private float diveSpeed = 1.0f;

    // 現在の状態
    private KeeperState currentState = KeeperState.STANDBY;

    // コンポーネント参照
    /* private KeepAnimController keepAnimController; */
    /* private Referee referee; */

    // 初期位置
    private Vector3 initPosition;
    private Vector3 initRotation;

    // KEEP状態用変数
    private Vector2 swipeStartPosition;
    private Vector2 swipeEndPosition;
    private SwipeDirection diveDirection;

    // Start is called before the first frame update
    void Start()
    {
        initPosition = this.transform.position;
        initRotation = this.transform.eulerAngles;
        /*keepAnimController = FindObjectOfType<KeepAnimController>();*/
        /*referee = FindObjectOfType<Referee>();*/

        // 初期状態を設定
        /* [DEBUG] */
        ChangeState(KeeperState.WAIT);
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
    private void EnterState(KeeperState state)
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
                // 防御アニメーション再生
                Dive();
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
    private void UpdateWait()
    {
        // パラメータの位置初期化を行う
        // タッチ入力を待つ
        if (Input.GetMouseButtonDown(0))
        {
            ChangeState(KeeperState.AIM);
        }
    }

    // AIM状態の更新
    private void UpdateAim()
    {
        // スワイプ終了（指/マウスを離した）
        if (Input.GetMouseButtonUp(0))
        {
            // スワイプの方向と強さを計算
            swipeEndPosition = Input.mousePosition;
            diveDirection = SwipeUtility.GetSwipeDirection(swipeStartPosition, swipeEndPosition);

            Debug.Log(diveDirection);

            ChangeState(KeeperState.GUARD);
        }
    }

    // GUARD状態の更新
    private void UpdateGuard()
    {
        // Refereeがスコアへ入ったら状態変更
        /*if (referee.IsScoreEntered())
        {
            ChangeState(KeeperState.NOTGUARDED);
        }
        else
        {
            // ボールが防御されたかチェック
            if (CheckBallGuarded())
            {
                ChangeState(KeeperState.GUARDED);
            }
        }*/
    }

    // GUARDED状態の更新
    private void UpdateGuarded()
    {

        // アニメーション終了後、STANDBY状態に戻る
        /*if (IsAnimationFinished())
        {
            ChangeState(KeeperState.STANDBY);
        }*/
    }

    // NOTGUARDED状態の更新
    private void UpdateNotGuarded()
    {
        // アニメーション終了後、STANDBY状態に戻る
        /*if (IsAnimationFinished())
        {
            ChangeState(KeeperState.STANDBY);
        }*/
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
                break;
            case KeeperState.GUARDED:
                break;
            case KeeperState.NOTGUARDED:
                break;
        }

        // 状態を更新
        currentState = newState;
    }

    // パラメータをリセット
    private void ResetParameters()
    {
        /* 位置の初期化 */
        this.transform.position = initPosition;
        this.transform.eulerAngles = initRotation;

        /* クラス固有パラメーターの初期化 */
        swipeStartPosition = Vector2.zero;
        swipeEndPosition = Vector2.zero;
        diveDirection = SwipeDirection.None;
        /*keepAnimController.StopKeep();*/
    }

    // ダイブ
    private void Dive()
    {
        
    }

    // WAIT状態にするためのパブリックメソッド
    public void SetToWAIT()
    {
        ChangeState(KeeperState.WAIT);
    }
}