using UnityEngine;
using UnityEngine.InputSystem;

namespace Sandbox.Kamada
{
    public class PlayerMove : MonoBehaviour
    {
        [SerializeField] InputAction moveAction;

        [SerializeField] Rigidbody rigidbody;

        [SerializeField] float moveSpeed = 1.0f;

        public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }

        private void OnEnable()
        {
            moveAction.Enable();
        }

        private void OnDisable()
        {
            moveAction.Disable();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            Vector2 input = moveAction.ReadValue<Vector2>();

            rigidbody.linearVelocity =
                new Vector3(input.x, rigidbody.linearVelocity.y, input.y);

            transform.rotation = new Quaternion(0.0f, 0.0f, 0.0f,0.0f);

        }
    }
}
