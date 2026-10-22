using UnityEngine;
using UnityEngine.InputSystem;

namespace Sandbox.Kamada
{
    public class PlayerJump : MonoBehaviour
    {
        [SerializeField] InputAction jumpAction;

        [SerializeField] Rigidbody rigidbody;

        [SerializeField] float jumpPower;

        bool isGroundFlag = false;

        public float JumpPower { get => jumpPower; set => jumpPower = value; }

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
            if (!isGroundFlag)
            {
                return;
            }

            if (jumpAction.WasPressedThisFrame())
            {
                rigidbody.linearVelocity = 
                    new Vector3(rigidbody.linearVelocity.x, jumpPower,rigidbody.linearVelocity.z);

                isGroundFlag = false;
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            isGroundFlag = true;
        }

        private void OnCollisionExit(Collision collision)
        {
            isGroundFlag = false;
        }
    }  

}

