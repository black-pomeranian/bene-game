using UnityEngine;

public enum SwipeDirection
{
    Left,
    UpperLeft,
    Up,
    UpperRight,
    Right,
    Down,
    None  // 有効なスワイプではない場合
}

public static class SwipeUtility
{
    // スワイプ方向を判定するための最小距離（これより短いスワイプは無視）
    private const float MIN_SWIPE_DISTANCE = 10f;

    /// <summary>
    /// スワイプの開始位置と終了位置から方向を判定する
    /// </summary>
    /// <param name="startPosition">スワイプ開始位置</param>
    /// <param name="endPosition">スワイプ終了位置</param>
    /// <returns>6方向のいずれか、または無効な場合はNone</returns>
    public static SwipeDirection GetSwipeDirection(Vector2 startPosition, Vector2 endPosition)
    {
        // スワイプベクトルを計算
        Vector2 swipeVector = endPosition - startPosition;

        // スワイプの長さが最小距離より短い場合は無視
        if (swipeVector.magnitude < MIN_SWIPE_DISTANCE)
        {
            return SwipeDirection.None;
        }

        // スワイプ角度を計算（ラジアン）
        float angle = Mathf.Atan2(swipeVector.y, swipeVector.x);

        // ラジアンから度に変換（-180°～180°）
        float angleDegrees = angle * Mathf.Rad2Deg;

        // 角度を0°～360°の範囲に変換
        if (angleDegrees < 0)
        {
            angleDegrees += 360f;
        }

        // 角度に基づいて方向を判定
        if (angleDegrees >= 330f || angleDegrees < 30f)
        {
            return SwipeDirection.Right;
        }
        else if (angleDegrees >= 30f && angleDegrees < 60f)
        {
            return SwipeDirection.UpperRight;
        }
        else if (angleDegrees >= 60f && angleDegrees < 120f)
        {
            return SwipeDirection.Up;
        }
        else if (angleDegrees >= 120f && angleDegrees < 150f)
        {
            return SwipeDirection.UpperLeft;
        }
        else if (angleDegrees >= 150f && angleDegrees < 210f)
        {
            return SwipeDirection.Left;
        }
        else // 210° ～ 330°
        {
            return SwipeDirection.Down;
        }
    }

    /// <summary>
    /// スワイプ方向からVector3を取得（3D空間での移動などに使用）
    /// </summary>
    /// <param name="direction">スワイプ方向</param>
    /// <returns>対応するVector3（x,z平面上の方向）</returns>
    public static Vector3 GetDirectionVector(SwipeDirection direction)
    {
        switch (direction)
        {
            case SwipeDirection.Left:
                return new Vector3(-1f, 0f, 0f);
            case SwipeDirection.UpperLeft:
                return new Vector3(-0.7f, 0f, 0.7f).normalized;
            case SwipeDirection.Up:
                return new Vector3(0f, 0f, 1f);
            case SwipeDirection.UpperRight:
                return new Vector3(0.7f, 0f, 0.7f).normalized;
            case SwipeDirection.Right:
                return new Vector3(1f, 0f, 0f);
            case SwipeDirection.Down:
                return new Vector3(0f, 0f, -1f);
            default:
                return Vector3.zero;
        }
    }

    /// <summary>
    /// タッチ入力からスワイプ方向を取得する便利なメソッド
    /// </summary>
    /// <param name="touch">Unityのタッチ情報</param>
    /// <returns>スワイプ方向</returns>
    public static SwipeDirection GetSwipeDirectionFromTouch(Touch touch)
    {
        if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            Vector2 startPos = touch.position - touch.deltaPosition;
            Vector2 endPos = touch.position;

            return GetSwipeDirection(startPos, endPos);
        }

        return SwipeDirection.None;
    }

    public static void CursorDisappear()
    {
        Cursor.visible = false;

        // ゲーム画面内にロック
        Cursor.lockState = CursorLockMode.Locked;
    }

    public static void CursorAppear()
    {
        Cursor.visible = true;

        // ゲーム画面内にロック解除
        Cursor.lockState = CursorLockMode.None;
    }

}