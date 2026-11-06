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
        /// プレイヤーが回転する速さ（1秒あたりの角度）
        /// </summary>
        [SerializeField] float rotateSpeed = 90f;

        Rigidbody rigidbody;
        IPlayerStatus status;

        /// <summary>
        /// Updateで取得した回転入力を保持する
        /// </summary>
        float rotateInput;

        void Awake()
        {
            // 同じGameObjectについているRigidbodyを取得する
            rigidbody = GetComponent<Rigidbody>();

            // IPlayerStatusを実装したコンポーネントを取得する
            if (!TryGetComponent(out status))
            {
                Debug.LogError( $"{name} に IPlayerStatus を実装したコンポーネントがありません",this);
            }
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
            // 左右の旋回入力を取得する
            rotateInput = rotateAction.action.ReadValue<Vector2>().x;
        }

        void FixedUpdate()
        {
            Move();
            Rotate();
        }

        void Move()
        {
            // プレイヤーが現在向いている方向を取得する
            Vector3 moveDirection = transform.forward;

            // 移動方向と移動速度から移動距離を計算して移動する
            rigidbody.linearVelocity = moveDirection * status.MoveSpeed;
        }

        void Rotate()
        {
            // 入力値に応じて回転する角度を計算する
            // マイナスを付けることで回転方向を反転させる
            float rotation = -rotateInput * rotateSpeed * Time.fixedDeltaTime;

            // 現在のプレイヤーの向きに回転角度を加える
            Quaternion nextRotation =
                rigidbody.rotation * Quaternion.Euler(0f, rotation, 0f);

            // 計算した向きを反映する
            rigidbody.MoveRotation(nextRotation);
        }
    }
}