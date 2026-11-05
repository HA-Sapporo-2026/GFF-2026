using System;
using UnityEngine;

public abstract class BlockPartsBase : MonoBehaviour
{
    // 耐久値→英語でDurabilityらしいです。
    // 現在の耐久値
    public int Durability { get; private set; }
    // 参照するパーツのデータ
    public BlockPartsData Settings { get; private set; }

    /// <summary>
    /// 耐久値を減らす処理の関数
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(int damage)
    {
        // 当たったことを伝える
        OnHit();

        if(CanDamage())
        {
            Durability = Math.Max(Durability - damage, 0);

            OnDamaged(damage);

            if (Durability <= 0)
            {
                OnBroken();
            }
        }
    }

    /// <summary>
    /// 当たったことを伝える関数
    /// </summary>
    protected virtual void OnHit() { }
    
    /// <summary>
    /// ダメージを受けられるかを判断する関数
    /// </summary>
    /// <returns></returns>
    protected virtual bool CanDamage() { return true; }

    /// <summary>
    /// ダメージを受けた時の処理の関数
    /// </summary>
    /// <param name="damage"></param>
    protected virtual void OnDamaged(int damage) { }

    /// <summary>
    /// 耐久値が0以下になったときの処理の関数
    /// </summary>
    protected virtual void OnBroken() { gameObject.SetActive(false); }
}
