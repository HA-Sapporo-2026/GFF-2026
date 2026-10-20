using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

namespace Sandbox.Gunji
{
    public class PlayerMove : MonoBehaviour
    {
        [SerializeField] float moveSpeed = 2.0f;

        // 移動速度
        public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }


        // 物理移動用のコンポーネント
        [SerializeField] Rigidbody rigidbody;

        // 移動用の入力設定
        [SerializeField] InputAction moveAction;

        void OnEnable()
        {
            // 移動入力の有効化
            moveAction.Enable();
        }

        void OnDisable()
        {
            // 移動入力の無効化
            moveAction.Disable();
        }

        // Update is called once per frame
        void Update()
        {
            // 移動入力値を取得
            Vector2 input = moveAction.ReadValue<Vector2>();

            Vector3 moveInput = new Vector3
            (
                input.x * moveSpeed,
                rigidbody.linearVelocity.y,
                input.y * moveSpeed
            );

            // Rigidbodyの移動方向と速度を扱う変数に、
            // 移動入力の値（方向）に速度をかけた値を渡す
            rigidbody.linearVelocity = moveInput ;
        }
    }

}