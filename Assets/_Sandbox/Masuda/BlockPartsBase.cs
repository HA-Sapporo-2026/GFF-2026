using System;
using UnityEngine;

public abstract class BlockPartsBase : MonoBehaviour
{
    // 耐久値→英語でDurabilityらしいです。
    // 現在の耐久値
    public int Durability { get; private set; }
    // 参照するパーツのデータ
    public BlockPartsData Settings { get; private set; }

    // 耐久値を減らす処理の関数
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

    protected virtual void OnHit() { }
    
    protected virtual bool CanDamage() { return true; }

    // ダメージを受けた時の処理の関数
    protected virtual void OnDamaged(int damage) { }

    // 耐久値が0以下になったときの処理の関数
    protected virtual void OnBroken() { gameObject.SetActive(false); }
}
