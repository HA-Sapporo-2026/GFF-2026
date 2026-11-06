using UnityEngine;
using UnityEngine.InputSystem;

namespace GFF2026.InGame
{
    public class PlayerMove : MonoBehaviour
    {
        /// <summary>
        /// プレイヤーの旋回入力を取得するInputAction
        /// キーボードのA/D、コントローラーの左スティックを取得する
        /// </summary>
        [SerializeField] InputActionReference rotateAction;

        /// <summary>
        /// プレイヤーが回転する速さ(1秒あたりの角度)
        /// </summary>
        [SerializeField] float rotateSpeed = 90f;

        Rigidbody rigidbody;
        PlayerMoveStatus playerMoveStatus;

        /// <summary>
        /// Updateで取得した回転入力を保持する
        /// </summary>
        float rotateInput;

        void Awake()
        {
            // 同じGameObjectについているRigidbodyを取得する
            rigidbody = GetComponent<Rigidbody>();

            // 移動速度を管理するPlayerMoveStatusを取得する
            playerMoveStatus = GetComponent<PlayerMoveStatus>();
        }

        void OnEnable()
        {
            rotateAction.action.Enable();
        }

        void OnDisable()
        {
            rotateAction.action.Disable();
        }

        void Update()
        {
            rotateInput = rotateAction.action.ReadValue<Vector2>().x;
        }

        void FixedUpdate()
        {
            Move();
            Rotate();
        }

        void Move()
        {
            // プレイヤーの向いている方向に常に前進
            Vector3 moveDirection = rigidbody.rotation * Vector3.forward;

            // 現在の位置に、移動する方向と距離を足して移動先を計算する
            // 移動距離は「移動方向 × 移動速度 × 物理演算の更新間隔」で求める
            rigidbody.MovePosition(
                rigidbody.position +
                moveDirection * playerMoveStatus.MoveSpeed * Time.fixedDeltaTime
            );
        }

        void Rotate()
        {
            // 入力値に応じて回転する角度を計算する
            // マイナスを付けることで、画面から見た左右の操作を反転させる
            float rotation = -rotateInput * rotateSpeed * Time.fixedDeltaTime;

            // 現在のプレイヤーの向きに回転角度を加える
            Quaternion nextRotation =
                rigidbody.rotation * Quaternion.Euler(0f, rotation, 0f);

            // 計算した向きを反映する
            rigidbody.MoveRotation(nextRotation);
        }
    }
}