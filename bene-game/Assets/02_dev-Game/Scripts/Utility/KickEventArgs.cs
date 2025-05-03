using UnityEngine;

public class KickEventArgs : System.EventArgs
{
    public Vector3 Direction { get; private set; }
    public float Power { get; private set; }

    public KickEventArgs(Vector3 direction, float power)
    {
        Direction = direction;
        Power = power;
    }
}
