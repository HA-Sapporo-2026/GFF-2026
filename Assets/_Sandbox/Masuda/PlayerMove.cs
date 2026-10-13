using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    public Rigidbody rb;

    float moveSpeed = 5.0f;

    public InputAction moveAction;


    
    private void OnEnable()
    {
        moveAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();

        rb.linearVelocity = new Vector3(moveInput.x, 0.0f, moveInput.y);
    }
}
