// Last assigned by 丹羽
using System.Collections.Generic;
using UnityEngine;

namespace GFF2026.InGame
{
    public class PlayerAttack : MonoBehaviour
    {
        //腕の種類をenum
        enum ArmType
        {
            Right,
            Left
        }

        //攻撃の間隔を計測する変数
        float attackTime;

        //IPlayerStatusを継承
        IPlayerStatus status;

        //攻撃用のコライダーを取得
        [SerializeField] Collider punchCollider;

        private void Awake()
        {
            if (!TryGetComponent(out status))
            {
                Debug.LogError($"{name} に IPlayerStatus を実装したコンポーネントがありません", this);
            }
        }

        private void Update()
        {
            //攻撃時間を計測
            attackTime += Time.deltaTime;

            //経過時間がAttackSpeedを超えたら腕を振る処理
            if(attackTime >= status.AttackSpeed)
            {
                punchCollider.enabled = true;

                Debug.Log("攻撃しました");
                
                //経過時間をリセット
                attackTime = 0;
            }
            else
            {
                punchCollider.enabled = false;
            }
        }

        //腕にIDamageableコンポーネントを継承しているオブジェクトが当たった時の処理
        private void OnTriggerEnter(Collider other)
        {
            //腕に当たったオブジェクトがIDamageableを持っているか検知
            if (other.TryGetComponent<IDamageable>(out var damageable))
            {
                //TakeDamage関数にAttackPowerの数値を送る
                damageable.TakeDamage(status.AttackPower);

                //攻撃の判定を無効にする
                punchCollider.enabled = false;
            }
        }
    }
}
