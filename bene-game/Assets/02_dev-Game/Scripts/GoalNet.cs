using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalNet : MonoBehaviour
{
    [SerializeField] private Referee referee;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            referee.NotifyGoal();
        }
    }
}
