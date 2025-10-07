using UnityEngine;

public class GameUtility:MonoBehaviour
{
    [SerializeField] GameObject sound;
    private bool isSoundOn = true;

    private void Start()
    {
        SwipeUtility.CursorDisappear();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (!Cursor.visible)
            {
                isSoundOn = false;
                SwipeUtility.CursorAppear();
            }
            else
            {
                isSoundOn = true;
                SwipeUtility.CursorDisappear();
            }

            sound.SetActive(isSoundOn);
        }
    }
}
