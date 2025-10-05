using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KickerTrackBall : Kicker
{
    // シリアライザブル
    [SerializeField] private float moveThreshold = 50f;
    [SerializeField] private float inputAngleRange = 120f;
    [SerializeField] private float maxAngle = 30f;
    [SerializeField] private float xSensitivity = 0.5f;
    [SerializeField] private float minHeight = 0.1f;
    [SerializeField] private float maxHeight = 1.5f;
    [SerializeField] private float heightSensitivity = 0.1f;
    [SerializeField] private float timingCycle = 2f;
    [SerializeField] private float justTiming = 0.3f;
    [SerializeField] private GameObject timingCircle;
    [SerializeField] private GameObject justCircle;
    [SerializeField] private GameObject stoppingCircle;

    private Vector2 accumulatedDelta;
    private float timingValue;

    // 状態に入るときの処理
    protected override void EnterState(KickerState state)
    {
        if (state == KickerState.STANDBY)
        {
            timingCircle.SetActive(false);
            justCircle.SetActive(false);
            stoppingCircle.SetActive(false);
        }
        else if (state == KickerState.AIM)
        {
            timingCircle.SetActive(true);
            justCircle.SetActive(true);
            justCircle.GetComponent<RectTransform>().localScale = Vector3.one * justTiming;
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
            justCircle.SetActive(false);
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

            // 入力ベクトル
            Vector3 inputDir = new Vector3(-accumulatedDelta.x * xSensitivity, 0, -accumulatedDelta.y * xSensitivity);

            // 入力方向の角度を取得
            float inputAngle = Vector3.SignedAngle(Vector3.back, inputDir.normalized, Vector3.up);

            // 入力角度（±inputAngleRange）→ ゲーム角度（±maxAngle）にマッピング
            float normalized = Mathf.Clamp(inputAngle / inputAngleRange, -1f, 1f); // -1〜1 に正規化
            float mappedAngle = normalized * maxAngle; // ±maxAngle にスケーリング

            // 方向ベクトルを生成
            Vector3 direction = Quaternion.Euler(0, mappedAngle, 0) * Vector3.back;

            // 高さ・速度計算
            float heightFactor = Mathf.Clamp(swipeSpeed * heightSensitivity, minHeight, maxHeight);
            float timingMultiplier = GetTimingMultiplier();

            aimVector3 = new Vector3(direction.x, heightFactor, direction.z) * timingMultiplier;

            ChangeState(KickerState.KICK);
        }
    }

    private void UpdateTiming()
    {
        float t = Mathf.Repeat(Time.time / timingCycle, 1f);

        // 二次曲線（Ease In）: ゆっくり始まって速く縮む
        float curvedT = t * t;

        timingValue = Mathf.Lerp(1.6f, 0.2f, curvedT);

        if (timingCircle != null)
        {
            timingCircle.GetComponent<RectTransform>().localScale = Vector3.one * timingValue;
        }
    }

    private float GetTimingMultiplier()
    {
        float diff = Mathf.Abs(timingValue - justTiming);
        return Mathf.Lerp(maxKickForce, minKickForce, diff * 2f);
    }
}