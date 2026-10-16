using UnityEngine;
using UnityEngine.InputSystem;

namespace Sandbox.Masuda
{
    public class PlayerDash : MonoBehaviour
    {
        [SerializeField] InputAction dashAction;

        [SerializeField] Rigidbody rigidbody;

        float normalSpeed = 1.0f;

        [SerializeField] float dashSpeed = 5.0f;

        float normalJump = 1.0f;

        [SerializeField] float dashJump = 10.0f;

        PlayerMove playerMove;

        PlayerJump playerJump;

        void Start()
        {
            playerMove = GetComponent<PlayerMove>();

            playerJump = GetComponent<PlayerJump>();

            normalSpeed = playerMove.MoveSpeed;

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
            if (dashAction.IsPressed())
            {
                playerMove.MoveSpeed = dashSpeed;
                playerJump.JumpPower = dashJump;
            }
            else
            {
                playerMove.MoveSpeed = normalSpeed;
                playerJump.JumpPower = normalJump;
            }
        }
    }
}