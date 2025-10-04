using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeeperTrackBall : Keeper
{
    [SerializeField] private float moveThreshold = 50f;

    private Vector2 accumulatedDelta;
    private Vector2 swipeStart;
    private Vector2 swipeEnd;

    protected override void EnterState(KeeperState state)
    {
        if (state == KeeperState.AIM)
        {
            swipeStart = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            accumulatedDelta = Vector2.zero;
        }
        else
        {
            base.EnterState(state);
        }
    }

    protected override void UpdateWait()
    {
        Vector2 delta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        // ちょっとでも動いたら AIM 開始
        if (delta.sqrMagnitude > 0.01f)
        {
            ChangeState(KeeperState.AIM);
        }
    }

    protected override void UpdateAim()
    {
        // トラックボール移動の蓄積
        Vector2 delta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        accumulatedDelta += delta;

        // 一定閾値を超えたらキック方向を決定して GUARD へ
        if (accumulatedDelta.magnitude > moveThreshold)
        {
            swipeEnd = accumulatedDelta;

            diveDirection = SwipeUtility.GetSwipeDirection(swipeStart, swipeEnd);

            if (isSideRevert)
            {
                diveDirection = RevertDirection(diveDirection);
            }

            ChangeState(KeeperState.GUARD);
        }
    }
}
