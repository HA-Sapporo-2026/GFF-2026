// Last assigned by 福野

namespace GFF2026.InGame
{
    /// <summary>
    /// ダメージを受けることができる、という概念を表現するインターフェース。
    /// </summary>
    /// <remarks>
    /// ダメージを受けることができるブロックパーツはこれを継承し、
    /// ダメージを与える側のプレイヤーはこのインターフェース経由でダメージを与える。
    /// </remarks>
    public interface IDamageable
    {
        /// <summary>
        /// ダメージを受ける処理。
        /// </summary>
        /// <param name="damage">受けるダメージ量</param>
        void TakeDamage(int damage);
    }

}