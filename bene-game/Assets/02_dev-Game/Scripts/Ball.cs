using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody rb;
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
        }
    }
}
