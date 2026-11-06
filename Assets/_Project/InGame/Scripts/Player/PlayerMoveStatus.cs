using UnityEngine;

public class PlayerMoveStatus : MonoBehaviour
{
    public float MoveSpeed { get; private set; } = 5f;

    public void SetMoveSpeed(float speed)
    {
        MoveSpeed = speed;
    }
}
