using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KickerTrackBall : Kicker
{
    // シリアライザブル
    [SerializeField] private float moveThreshold = 50f;
    [SerializeField] private float minWidth = -10.0f;
    [SerializeField] private float maxWidth = 10.0f;
    [SerializeField] private float xSensitivity = 0.5f;
    [SerializeField] private float minHeight = 0.1f;
    [SerializeField] private float maxHeight = 1.5f;
    [SerializeField] private float heightSensitivity = 0.1f;
    [SerializeField] private float timingCycle = 2f;
    [SerializeField] private GameObject timingCircle;
    [SerializeField] private GameObject stoppingCircle;

    private Vector2 accumulatedDelta;
    private float timingValue;

    // 状態に入るときの処理
    protected override void EnterState(KickerState state)
    {
        if (state == KickerState.STANDBY)
        {
            timingCircle.SetActive(false);
            stoppingCircle.SetActive(false);
        }
        else if (state == KickerState.AIM)
        {
            timingCircle.SetActive(true);
            accumulatedDelta = Vector2.zero;
            swipeStartTime = Time.time;
        }
        base.EnterState(state);
    }

    protected override void ExitState(KickerState state)
    {
        if(state == KickerState.AIM)
        {
            timingCircle.SetActive(false);
            stoppingCircle.SetActive(true);
            stoppingCircle.GetComponent<RectTransform>().localScale = Vector3.one * timingValue;
        }
        else if (state == KickerState.KICK)
        {
            stoppingCircle.SetActive(false);
        }
        base.ExitState(state);
    }

    // WAIT状態の更新
    protected override void UpdateWaitState()
    {
        // とりあえず何も待たずに遷移
        ChangeState(KickerState.AIM);
    }

    // AIM状態の更新
    protected override void UpdateAimState()
    {
        // トラックボール入力（マウス移動量）
        Vector2 delta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        accumulatedDelta += delta;

        UpdateTiming();

        if (accumulatedDelta.magnitude > moveThreshold)
        {
            float elapsed = Time.time - swipeStartTime;
            float swipeSpeed = accumulatedDelta.magnitude / (elapsed + 0.001f);

            float adjustedDelta_x = Mathf.Clamp(-accumulatedDelta.x * xSensitivity, minWidth, maxWidth);
            float adjustedDelta_z = -accumulatedDelta.y;

            Debug.Log("accumulatedDelta_x: " + -accumulatedDelta.x);
            Debug.Log("adjustedDelta_x: " + adjustedDelta_x);

            Vector3 direction = new Vector3(adjustedDelta_x, 0, adjustedDelta_z).normalized;
            if (direction.z > 0) direction.z = -0.1f;

            direction.Normalize();

            float heightFactor = Mathf.Clamp(swipeSpeed * heightSensitivity, minHeight, maxHeight);

            float timingMultiplier = GetTimingMultiplier();

            aimVector3 = new Vector3(direction.x, heightFactor, direction.z) * timingMultiplier;

            ChangeState(KickerState.KICK);
        }
    }

    private void UpdateTiming()
    {
        float t = Mathf.Repeat(Time.time / timingCycle, 1f);

        timingValue = Mathf.Lerp(1f, 0.2f, t);

        if (timingCircle != null)
        {
            timingCircle.GetComponent<RectTransform>().localScale = Vector3.one * timingValue;
        }
    }

    private float GetTimingMultiplier()
    {
        float diff = Mathf.Abs(timingValue - 0.5f);
        return Mathf.Lerp(maxKickForce, minKickForce, diff * 2f);
    }
}