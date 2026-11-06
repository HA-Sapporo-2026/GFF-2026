// Last assigned by 増田
using System;
using UnityEngine;

namespace GFF2026.InGame
{
    public abstract class BlockPartsBase : MonoBehaviour, IDamageable
    {
        [SerializeField] BlockPartsData data;

        /// <summary>
        /// 自身が参照しているデータ。
        /// </summary>
        public BlockPartsData Data => data;

        /// <summary>
        /// 現在の耐久値。
        /// </summary>
        public int Durability { get; private set; }

        /// <summary>
        /// 初期化処理。
        /// </summary>
        void Awake()
        {
            // 自身の耐久値を初期値に設定する
            Durability = data.Durability;
        }

        /// <summary>
        /// 耐久値を減らす処理の関数。
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(int damage)
        {
            // 自身に当たったことを伝える
            OnHit();

            // ダメージを受けることができない場合は以降の処理はしない
            if (!CanDamage()) { return; }

            // 現在の耐久値を減らし、ダメージを受けたことを自身に伝える
            Durability = Math.Max(Durability - damage, 0);
            OnDamaged(damage);
            
            // 耐久値が0以下になった場合、ブロックが壊れたことを伝える
            if (Durability <= 0) { OnBroken(); }
        }

        /// <summary>
        /// 当たったことを伝える関数。
        /// </summary>
        protected virtual void OnHit()
        {
        }

        /// <summary>
        /// ダメージを受けられるかを判断する関数。
        /// </summary>
        /// <returns></returns>
        protected virtual bool CanDamage()
        {
            return true;
        }

        /// <summary>
        /// ダメージを受けた時の処理の関数。
        /// </summary>
        /// <param name="damage">自身に与えたダメージ</param>
        protected virtual void OnDamaged(int damage)
        {
        }

        /// <summary>
        /// 耐久値が0以下になったときの処理の関数。
        /// </summary>
        protected virtual void OnBroken()
        {
            gameObject.SetActive(false);
        }
    }

}