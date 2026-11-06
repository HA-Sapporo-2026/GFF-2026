using System.Collections.Generic;
using UnityEngine;

namespace GFF2026.InGame
{
    public class PlayerAttack : MonoBehaviour
    {
        //攻撃の間隔を計測する変数
        float attackTime;
        //振る腕を決める変数
        int armNumber;

        //IPlayerStatusを継承
        IPlayerStatus status;

        //右腕のコライダーを取得
        [SerializeField] Collider rightArmCollider;
        //左腕コライダーを取得
        [SerializeField] Collider leftArmCollider;

        private void Awake()
        {
            if (!TryGetComponent(out status))
            {
                Debug.LogError($"{name} に IPlayerStatus を実装したコンポーネントがありません", this);
            }

            //初めに振る腕を右手に設定
            armNumber = 0;
        }

        private void Update()
        {
            //攻撃時間を計測
            attackTime += Time.deltaTime;

            //経過時間がAttackSpeedを超えたら腕を振る処理
            if(attackTime >= status.AttackSpeed)
            {
                switch (armNumber)
                {
                    //右腕の処理
                    case 0:
                        //左手の当たり判定を無効にする
                        leftArmCollider.enabled = false;
                        //右手の当たり判定を有効にする
                        rightArmCollider.enabled = true;
                        //次回の時間経過時の振る腕を左手にする
                        armNumber = 1;
                        break;
                    
                    case 1:
                        //右手の当たり判定を無効にする
                        rightArmCollider.enabled = false;
                        //左手の当たり判定を有効にする
                        leftArmCollider.enabled = true;
                        //次回の時間経過時の振る腕を右手にする
                        armNumber = 0;
                        break;
                }

                //経過時間をリセット
                attackTime = 0;
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

                //腕の判定を無効にする
                rightArmCollider.enabled = false;
                leftArmCollider.enabled = false;
            }
        }
    }
}
