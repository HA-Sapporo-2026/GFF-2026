using UnityEngine;
using UnityEngine.InputSystem;

namespace Sandbox.Masuda
{
    public class PlayerDash : MonoBehaviour
    {
        [SerializeField] InputAction dashAction;

        [SerializeField] Rigidbody rigidbody;

        // 通常状態の速さ
        float normalSpeed = 1.0f;
        // ダッシュ状態の速さ
        [SerializeField] float dashSpeed = 5.0f;
        // 通常状態のジャンプ力
        float normalJump = 1.0f;
        // ダッシュ状態のジャンプ力
        [SerializeField] float dashJump = 10.0f;

        PlayerMove playerMove;

        PlayerJump playerJump;

        void Start()
        {
            playerMove = GetComponent<PlayerMove>();

            playerJump = GetComponent<PlayerJump>();

            // playerMoveから、通常状態の速さをもらう
            normalSpeed = playerMove.MoveSpeed;
            // 同様に、playerJumpから通常状態のジャンプ力をもらう
            normalJump = playerJump.JumpPower;
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
            // ダッシュ用の入力があった場合
            if (dashAction.IsPressed())
            {
                // それぞれのスクリプトにある速さとジャンプの変数に、
                // ダッシュ状態の値を入れる
                playerMove.MoveSpeed = dashSpeed;
                playerJump.JumpPower = dashJump;
            }
            // そうでない場合は、通常状態の速さとジャンプの値を入れる
            else
            {
                playerMove.MoveSpeed = normalSpeed;
                playerJump.JumpPower = normalJump;
            }
        }
    }
}