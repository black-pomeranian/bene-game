using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickerCpu : Kicker
{
    [SerializeField] private float waitMaxTime = 4.0f;
    [SerializeField] private float waitMinTime = 2.0f;

    private float waitingTime = 0.0f;

    // 状態に入るときの処理
    protected override void EnterState(KickerState state)
    {
        if (state == KickerState.AIM)
        {
            // AIMのときCPUは何もしない
        }
        else
        {
            // AIM以外は親クラスの処理をそのまま使う
            base.EnterState(state);
            if (state == KickerState.WAIT)
            {
                waitingTime = 0.0f;
            }
        }
    }

    // WAIT状態の更新
    protected override void UpdateWaitState()
    {
        waitingTime += Time.deltaTime;
        float waitTime = Random.Range(waitMinTime, waitMaxTime);

        // タッチ/クリック入力検出
        if (waitingTime >= waitTime)
        {
            ChangeState(KickerState.AIM);
        }
    }

    // AIM状態の更新
    protected override void UpdateAimState()
    {
        KickTarget target = kickTargets[Random.Range(0, kickTargets.Count)];
        Vector3 directionToTarget = (target.target.position - ball.rb.transform.position).normalized;
        float kickForce = Random.Range(minKickForce, maxKickForce);

        aimVector3 = directionToTarget * kickForce;

        if (target == kickTargets[0] || target == kickTargets[4] || target == kickTargets[5])
        {
            aimVector3 = new Vector3(aimVector3.x, aimVector3.y*0.1f, aimVector3.z);
        }

        ChangeState(KickerState.KICK);
    }
}