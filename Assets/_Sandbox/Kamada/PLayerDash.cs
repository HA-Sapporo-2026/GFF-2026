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

        float jumpPower;

        float moveSpeed;

        // 移動スピードの上昇倍率
        [SerializeField] float speedMagnification = 2.0f;

        // ジャンプパワーの上昇倍率
        [SerializeField] float jumpMagnification = 2.0f;

        private void Start()
        {
            playerJump = GetComponent<PlayerJump>();

            playerMove = GetComponent<PlayerMove>();

            jumpPower = playerJump.JumpPower;

            moveSpeed = playerMove.MoveSpeed;
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
                playerMove.MoveSpeed  = moveSpeed * speedMagnification;

                playerJump.JumpPower  = jumpPower * jumpMagnification;

                return;
            }

            playerMove.MoveSpeed = moveSpeed;

            playerJump.JumpPower = jumpPower;
        }
    }
}
