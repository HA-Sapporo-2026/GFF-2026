using UnityEngine;
using UnityEngine.InputSystem;

namespace Sandbox.Chiba
{
    public class PlayerJump : MonoBehaviour
    {
        public InputAction jumpAction;
        public Rigidbody rigidbody;
        // 床を検知するトリガーの当たり判定
        public Collider jumpTrigger;
        // ジャンプ力
        public float jumpHeight = 5;
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
            if (jumpAction./*IsPressed()*/WasPressedThisFrame())
            {
                rigidbody.linearVelocity += new Vector3(0, jumpHeight - rigidbody.linearVelocity.y, 0);
            }
        }
    }
}
