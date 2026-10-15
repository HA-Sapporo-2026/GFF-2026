using UnityEngine;
using UnityEngine.InputSystem;

namespace Sandbox.Masuda
{
    public class PlayerJump : MonoBehaviour
    {
        [SerializeField] Rigidbody rigidbody;

        [SerializeField] float jumpPower = 5.0f;

        public float JumpPower { get => jumpPower; set => jumpPower = value; }

        [SerializeField] InputAction jumpAction;

        bool isGround = true;

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
            if(jumpAction.WasPressedThisFrame() && isGround)
            {
                rigidbody.linearVelocity = new Vector3(rigidbody.linearVelocity.x, jumpPower, rigidbody.linearVelocity.z);

                isGround = false;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            isGround = true;
        }

        private void OnCollisionExit(Collision collision)
        {
            isGround = false;
        }
    }
}

