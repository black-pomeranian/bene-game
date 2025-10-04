using UnityEngine;

public class GameUtility:MonoBehaviour
{

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            SwipeUtility.CursorAppear();
        }
    }

}
