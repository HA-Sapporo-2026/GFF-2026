using UnityEngine;
using UnityEngine.InputSystem;

namespace Sandbox.Chiba
{
    public class PlayerJump : MonoBehaviour
    {
        [SerializeField]InputAction jumpAction;
        [SerializeField]Rigidbody rigidbody;
        // ジャンプ力
        [SerializeField]float jumpHeight = 5;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void OnEnable()
        {
            jumpAction.Enable();
        }
        private void OnDisable()
        {
            jumpAction.Disable();
        }
        private void OnCollisionStay(Collision collision)
        {
            if (jumpAction.WasPressedThisFrame())
            {
                Vector3 velocity = rigidbody.linearVelocity;
                velocity.y = jumpHeight;
                rigidbody.linearVelocity = velocity;
            }
        }
    }
}
