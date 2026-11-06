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
        //腕のコライダーを管理するリスト
        List<Collider> childColliders = new List<Collider>();

        //右腕を取得
        [SerializeField] GameObject rightArm;
        //左腕を取得
        [SerializeField] GameObject leftArm;

        private void Awake()
        {
            if (!TryGetComponent(out status))
            {
                Debug.LogError($"{name} に IPlayerStatus を実装したコンポーネントがありません", this);
            }

            // 子オブジェクトのColliderを取得
            foreach (Transform child in transform)
            {
                Collider collider = child.GetComponent<Collider>();

                if (collider != null)
                {
                    childColliders.Add(collider);
                }
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
                        childColliders[1].enabled = false;
                        //右手の当たり判定を有効にする
                        childColliders[0].enabled = true;
                        //次回の時間経過時の振る腕を左手にする
                        armNumber = 1;
                        break;
                    
                    case 1:
                        //右手の当たり判定を無効にする
                        childColliders[0].enabled = false;
                        //左手の当たり判定を有効にする
                        childColliders[1].enabled = true;
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
                childColliders[0].enabled = false;
                childColliders[1].enabled = false;
            }
        }
    }
}
