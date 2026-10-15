using Sandbox.Chiba;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Sandbox.Kamada
{
    public class PLayerDash : MonoBehaviour
    {
        [SerializeField] InputAction dashAction;

        [SerializeField] Rigidbody rigidbody;

        PlayerJump playerJump;

        PlayerMove playerMove;

        float initJumpPower = 0;

        float initMoveSpeed = 0;

        [SerializeField] float speedMagnification = 2.0f;

        [SerializeField] float jumpMagnification = 2.0f;

        private void Start()
        {
            playerJump = GetComponent<PlayerJump>();

            playerMove = GetComponent<PlayerMove>();

            initJumpPower = playerJump.JumpPowerGet();

            initMoveSpeed = playerMove.MoveSpeedGet();
        }

        private void OnEnable()
        {
            dashAction.Enable();
        }

        private void OnDisable()
        {
            dashAction.Disable();
        }

        // Update is called once per frame
        void Update()
        {
            if (dashAction.IsPressed())
            {
                playerMove.MoveSpeedSet(initMoveSpeed * speedMagnification);

                playerJump.JumpPowerSet(initJumpPower * jumpMagnification);

                return;
            }

            playerMove.MoveSpeedSet(initMoveSpeed);
            playerJump.JumpPowerSet(initJumpPower);
        }
    }
}
