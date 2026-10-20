using UnityEngine;
using UnityEngine.InputSystem;

namespace Sandbox.Chiba
{
    public class PlayerMove : MonoBehaviour
    {
        [SerializeField]InputAction moveAction;

        [SerializeField]Rigidbody rigidbody;
        [SerializeField]float speed;
        PlayerDash dash;

        public Vector2 MoveInput { get; private set; }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            dash = this.GetComponent<PlayerDash>();
        }

        void OnEnable()
        {
            moveAction.Enable();
        }

        void OnDisable()
        {
            moveAction.Disable();
        }

        // Update is called once per frame
        void Update()
        {
            MoveInput = moveAction.ReadValue<Vector2>();
            rigidbody.linearVelocity = new Vector3(MoveInput.x * speed * dash.dashSpeed, rigidbody.linearVelocity.y, MoveInput.y * speed * dash.dashSpeed) /** Time.deltaTime*/;
        }
    }

}