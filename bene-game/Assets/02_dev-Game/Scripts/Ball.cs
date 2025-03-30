using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void AddForce(Vector3 force)
    {
        if (rb != null)
        {
            rb.AddForce(force, ForceMode.Impulse);
        }
    }
}
