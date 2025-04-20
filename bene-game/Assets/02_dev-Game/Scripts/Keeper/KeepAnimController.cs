using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepAnimController : MonoBehaviour
{
    private Animator _animator;
    private float _height;
    private float _turn;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void GetDiveParam(SwipeDirection diveDirection)
    {
        switch (diveDirection)
        {
            case SwipeDirection.Left:
                _height = 0.0f;
                _turn = -1.0f;
                break;
            case SwipeDirection.UpperLeft:
                _height = 1.0f;
                _turn = -1.0f;
                break;
            case SwipeDirection.Up:
                _height = 1.0f;
                _turn = 0.0f;
                break;
            case SwipeDirection.UpperRight:
                _height = 1.0f;
                _turn = 1.0f;
                break;
            case SwipeDirection.Right:
                _height = 0.0f;
                _turn = 1.0f;
                break;
            case SwipeDirection.None:
                _height = 0.0f;
                _turn = 0.0f;
                break;
        }
    }

    public void PlayDiveAnim(SwipeDirection diveDerection)
    {
        GetDiveParam(diveDerection);
        
        _animator.SetFloat("Height", _height);
        _animator.SetFloat("Turn", _turn);

        _animator.SetTrigger("Dive");
    }
    public void PlayDiveExitAnim()
    {
        _animator.SetTrigger("Exit");
    }
}
