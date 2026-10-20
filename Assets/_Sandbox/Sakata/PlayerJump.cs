using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Sandbox.Sakata
{
    public class PlayerJump : MonoBehaviour
    {
        [SerializeField] Rigidbody rigidbody;
        [SerializeField] Collider collider;
        [SerializeField] InputAction jumpAction;
        [SerializeField] PlayerDash playerDash;
        [SerializeField] PlayerMove playerMove;
        [SerializeField] float jumpPower = 5.0f;
        [SerializeField] float dashJumpMultiplier = 1.5f;

        bool switchFloor;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void OnEnable()
        {
            jumpAction.Enable();
        }

        void OnDisable()
        {
            jumpAction.Disable();
        }

        void Update()
        {
            if (jumpAction.WasPressedThisFrame() && !switchFloor)
            {
                if (playerDash.dashFlag && playerMove.moveFlag)
                    rigidbody.linearVelocity = new Vector3(rigidbody.linearVelocity.x, jumpPower * dashJumpMultiplier, rigidbody.linearVelocity.y);
                else
                    rigidbody.linearVelocity = new Vector3(rigidbody.linearVelocity.x, jumpPower, rigidbody.linearVelocity.y);
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            switchFloor = false;
        }

        private void OnCollisionExit(Collision collision)
        {
            switchFloor = true;
        }
    }
}