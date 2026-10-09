using UnityEngine;
using UnityEngine.InputSystem;

namespace Sandbox.Mutou
{
    public class PlayerMove : MonoBehaviour
    {
        [SerializeField] private InputActionReference moveAction;
        [SerializeField] float moveSpeed;
        [SerializeField] Rigidbody rigidBody;
        private Vector2 moveInput;

        private void OnEnable()
        {
            moveAction.action.started += OnMove;
            moveAction.action.performed += OnMove;
            moveAction.action.canceled += OnMove;

            moveAction.action.Enable();
        }
        private void OnDisable()
        {
            moveAction.action.started -= OnMove;
            moveAction.action.performed -= OnMove;
            moveAction.action.canceled -= OnMove;

            moveAction.action.Disable();
        }
        public void OnMove(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
        }

        private void FixedUpdate()
        {

            Vector3 move =
                transform.forward * moveInput.y +
                transform.right * moveInput.x;

            rigidBody.MovePosition(
                rigidBody.position +
                move.normalized * moveSpeed);
        }
    }
}