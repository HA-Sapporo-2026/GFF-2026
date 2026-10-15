using UnityEngine;
using UnityEngine.InputSystem;

namespace Sandbox.Kamada
{
    public class PlayerJump : MonoBehaviour
    {
        [SerializeField] InputAction jumpAction;

        [SerializeField] Rigidbody rd;

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

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

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
                rd.linearVelocity = new Vector3(0.0f, jumpPower,0.0f);

                Debug.Log("A");
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

