using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] InputActionReference moveAction;
    [SerializeField] float moveSpeed = 5f;
    Rigidbody Rigidbody;

    void OnEnable()
    {
       moveAction.action.Enable(); 
    }

    void OnDisable()
    {
        moveAction.action.Disable();
    }

    void Start()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }

    
    void FixedUpdate()
    {
        Move();
    }
    void Move()
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>();

        Vector3 move =
            transform.right * input.x +
            transform.forward * input.y;

        Rigidbody.MovePosition(
            Rigidbody.position + move * moveSpeed * Time.deltaTime
        );
    }
}
