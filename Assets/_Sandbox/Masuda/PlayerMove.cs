using UnityEngine;
using UnityEngine.InputSystem;

namespace Sandbox.Masuda
{
    public class PlayerMove : MonoBehaviour
    {
        [SerializeField] Rigidbody rigidbody;

        [SerializeField]float moveSpeed = 5.0f;

        public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }

        [SerializeField] InputAction moveAction;
    
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

            rigidbody.linearVelocity = new Vector3(moveInput.x * moveSpeed, rigidbody.linearVelocity.y, moveInput.y * moveSpeed);
        }
    }

}