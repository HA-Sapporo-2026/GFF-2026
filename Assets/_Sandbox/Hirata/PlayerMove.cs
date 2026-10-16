using UnityEngine;
using UnityEngine.InputSystem;

namespace Sandbox.Hirata
{
    public class PlayerMove : MonoBehaviour
    {
        private StateMachine stateMachine = new StateMachine();
        [SerializeField] InputActionReference moveAction;
        [SerializeField] float moveSpeed = 5f;
        Rigidbody rigidbody;

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
            rigidbody = GetComponent<Rigidbody>();
        }

        void Update()
        {
            stateMachine.Update();
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

            rigidbody.MovePosition(
                rigidbody.position + move * moveSpeed
            );
        }
    }
}