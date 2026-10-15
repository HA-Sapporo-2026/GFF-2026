using UnityEngine;
using UnityEngine.InputSystem;

namespace Sandbox.Gunji
{
    public class PlayerJump : MonoBehaviour
    {
        public float jumpPower = 5.0f;

        public Rigidbody rigidbody;

        public InputAction jumpAction;

        bool isGrounded;

        void OnEnable()
        {
            // 移動入力の有効化
            jumpAction.Enable();
        }

        void OnDisable()
        {
            // 移動入力の無効化
            jumpAction.Disable();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (jumpAction.WasPressedThisFrame() && isGrounded)
            {
                isGrounded = false;

                rigidbody.AddForce(
                 Vector3.up * jumpPower,
                 ForceMode.Impulse
             );
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            // 地面に接触したらジャンプ可能にする           
            isGrounded = true;
           
        }

        void OnCollisionExit(Collision collision)
        {
            // 地面から離れたらジャンプ不可
            isGrounded = false;        
        }

    }

}
