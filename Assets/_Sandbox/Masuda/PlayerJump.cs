using UnityEngine;
using UnityEngine.InputSystem;

namespace Sandbox.Masuda
{
    public class PlayerJump : MonoBehaviour
    {
        public Rigidbody rb;

        [SerializeField] float jumpPower = 5.0f;
        public InputAction jumpAction;

        bool isJumping = false;

        private void OnEnable()
        {
            jumpAction.Enable();
        }

        private void OnDisable()
        {
            jumpAction.Disable();
        }

        // Update is called once per frame
        void Update()
        {
            if(jumpAction.WasPressedThisFrame() && !isJumping)
            {
                rb.linearVelocity += new Vector3(0.0f, jumpPower, 0.0f);

                isJumping = true;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            isJumping = false;
        }
    }
}

