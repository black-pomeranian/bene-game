using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeeperCpu : Keeper
{
    private float diveWaitTime = 1.0f;
    private float diveWaitingTime = 0.0f;
    private bool isKicked = false;

    [SerializeField] private float diveWaitOffset = 0.1f;

    protected override void EnterState(KeeperState state)
    {
        if (state == KeeperState.AIM)
        {
            diveWaitingTime = 0.0f;
        }
        else
        {
            base.EnterState(state);
            if (state == KeeperState.WAIT)
            {
                isKicked = false;
            }
        }
    }

    protected override void UpdateWait()
    {
        if (isKicked)
        {
            ChangeState(KeeperState.AIM);
        }
    }

    protected override void UpdateAim()
    {
        if (diveWaitingTime > diveWaitTime)
        {
            ChangeState(KeeperState.GUARD);
        }

        diveWaitingTime += Time.deltaTime;
    }

    public override void SetDiveInfoFromKick(SwipeDirection direction, float arrivalTime)
    {
        isKicked = true;
        diveDirection = direction;
        diveWaitTime = arrivalTime - diveWaitOffset;
    }
}
