// Last assigned by 丹羽
using System.Buffers;
using System.Collections.Generic;
using UnityEngine;

namespace GFF2026.InGame
{
    public class PlayerAttack : MonoBehaviour
    {
        //攻撃用のコライダーを取得
        [SerializeField] Collider punchCollider;

        //攻撃の間隔を計測する変数
        float attackTime;

        //IPlayerStatusを継承
        IPlayerStatus status;

        void Awake()
        {
            if (!TryGetComponent(out status))
            {
                Debug.LogError($"{name} に IPlayerStatus を実装したコンポーネントがありません", this);
            }
        }

        void Update()
        {
            //攻撃時間を計測
            attackTime += Time.deltaTime;

            //経過時間がAttackSpeedを超えたら腕を振る処理
            if (attackTime >= status.AttackSpeed)
            {
                punchCollider.enabled = true;

                Debug.Log("攻撃しました");

                //経過時間をリセット
                attackTime = 0;
                Punch();
            }
        }

        void Punch()
        {
            // 判定を行う座標、サイズを取得する
            var position = punchCollider.bounds.center;
            var extents = punchCollider.bounds.extents;

            // 当たり判定の結果を入れる配列を借りる
            var results = ArrayPool<Collider>.Shared.Rent(32);

            try
            {
                // コライダーの位置に球体の当たり判定を生成して衝突したオブジェクトの数と情報を取得する
                var hits = Physics.OverlapBoxNonAlloc(position, extents, results);

                IDamageable nearest = null;
                var nearestDistance = float.MaxValue;

                if (hits <= 0) { return; }

                // 衝突したオブジェクトの数分ループさせる
                for (var i = 0; i < hits; i++)
                {
                    var hit = results[i];
                    if (!hit.TryGetComponent<IDamageable>(out var damageable)) { continue; }

                    // 衝突したブロックパーツの位置とコライダーの位置との距離を算出する
                    var distance = Vector3.Distance(transform.position, hit.transform.position);

                    // 一番近いブロックパーツの距離よりも近いブロックパーツが見つかったら
                    // それをターゲットとする
                    if (distance < nearestDistance)
                    {
                        nearest = damageable;
                        nearestDistance = distance;
                    }
                }

                // 一番近いブロックパーツにダメージを与える
                nearest?.TakeDamage(status.AttackPower);
            }
            finally
            {
                // 借りたコライダーの配列を返す
                ArrayPool<Collider>.Shared.Return(results);
            }
        }
    }
}
