using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Sandbox.Sakata
{
    public class PlayerMove : MonoBehaviour
    {
        [SerializeField] Rigidbody rigidbody;
        [SerializeField] InputAction moveAction;
        [SerializeField] PlayerDash playerDash;
        public bool moveFlag;

        void OnEnable()
        {
            moveAction.Enable();
        }
        void OnDisable()
        {
            moveAction.Disable();
        }

        // Update is called once per frame
        void Update()
        {
            Vector2 moveInput = moveAction.ReadValue<Vector2>();

            // sqrMagnitudeでベクトルの長さを見ることができる。0.01fなどの適当な閾値とみて0以上なら入力があると判断できる
            if (moveInput.sqrMagnitude > 0.01f)
            {
                if (playerDash.dashFlag)
                {
                    moveFlag = true;
                    rigidbody.linearVelocity = new Vector3(moveInput.x * playerDash.dashSpeed, rigidbody.linearVelocity.y, moveInput.y * playerDash.dashSpeed);
                }
                else
                {
                    moveFlag = false;
                    rigidbody.linearVelocity = new Vector3(moveInput.x, rigidbody.linearVelocity.y, moveInput.y);
                }
            }
        }
    }
}