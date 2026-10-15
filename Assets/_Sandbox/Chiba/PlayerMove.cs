using UnityEngine;
using UnityEngine.InputSystem;

namespace Sandbox.Chiba
{
    public class PlayerMove : MonoBehaviour
    {
        [SerializeField]InputAction moveAction;

        [SerializeField]Rigidbody rigidbody;
        [SerializeField]float speed;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

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
            rigidbody.linearVelocity = new Vector3(moveInput.x * speed, rigidbody.linearVelocity.y, moveInput.y * speed) /** Time.deltaTime*/;
        }
    }

}