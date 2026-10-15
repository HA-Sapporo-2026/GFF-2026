using UnityEngine;
using UnityEngine.InputSystem;

namespace Sandbox.Chiba
{
    public class PlayerMove : MonoBehaviour
    {
        [SerializeField] public InputAction moveAction;

        [SerializeField]Rigidbody rigidbody;
        [SerializeField]float speed;
        PlayerDash dash;
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
            Vector2 moveInput = moveAction.ReadValue<Vector2>();
            rigidbody.linearVelocity = new Vector3(moveInput.x * speed * dash.dashSpeed, rigidbody.linearVelocity.y, moveInput.y * speed * dash.dashSpeed) /** Time.deltaTime*/;
        }
    }

}