using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{

    [SerializeField] private float torqueMultiplier = 10f; // トルクの強さ

    public Rigidbody rb {get; private set; } // Rigidbodyコンポーネント
    private Vector3 initPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        initPosition = this.transform.position;
    }

    public void Initialize()
    {
        // 速度と回転速度をリセット
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // 初期位置にセット
        this.transform.position = initPosition;
    }

    public void AddForce(Vector3 force)
    {
        if (rb != null)
        {
            rb.AddForce(force, ForceMode.Impulse);

             // トルクを加えて回転させる
            // 例えば進行方向と上方向から回転軸を作る:
            Vector3 torque = Vector3.Cross(force.normalized, Vector3.down) * torqueMultiplier;
            rb.AddTorque(torque, ForceMode.Impulse);
        }
    }
}
