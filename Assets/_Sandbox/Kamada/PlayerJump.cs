using UnityEngine;
using UnityEngine.InputSystem;

namespace Sandbox.Kamada
{
    public class PlayerJump : MonoBehaviour
    {
        [SerializeField] InputAction jumpAction;

        [SerializeField] Rigidbody rigidbody;

        public float jumpPower = 3.0f;

        bool isGroundFlag = false;

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
                rigidbody.linearVelocity = new Vector3(0.0f, jumpPower,0.0f);
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

