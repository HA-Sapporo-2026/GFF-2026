using UnityEngine;
using UnityEngine.InputSystem;


namespace Sandbox.Gunji
{
    public class PlayerDash : MonoBehaviour
    {
        [SerializeField] float dashSpeed = 4.0f;

        [SerializeField] float dashJumpPower = 10.0f;

        float speed = 0f;

        float jumpPower = 0f;

        [SerializeField] Rigidbody rigidbody;

        [SerializeField] InputAction dashAction;

        PlayerMove playerMove;

        PlayerJump playerJump;

        void OnEnable()
        {
            // 移動入力の有効化
            dashAction.Enable();
        }

        void OnDisable()
        {
            // 移動入力の無効化
            dashAction.Disable();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            playerMove = GetComponent<PlayerMove>();
            playerJump = GetComponent<PlayerJump>();

             speed = playerMove.MoveSpeed;
             jumpPower = playerJump.JumpPower;
        }

        // Update is called once per frame
        void Update()
        {
            if (dashAction.IsPressed())
            {
                playerMove.MoveSpeed = dashSpeed;
                playerJump.JumpPower = dashJumpPower;
            }
            else
            {
                playerMove.MoveSpeed = speed;
                playerJump.JumpPower = jumpPower;
            }
        }
    }

}
