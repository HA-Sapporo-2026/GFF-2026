using UnityEngine;

public class BlockPartsBase : MonoBehaviour
{
    // 耐久値→英語でDurabilityらしいです。
    // 現在の耐久値
    public int Durability { get; private set; }
    // 最大の耐久値
    public int Max { get; private set; }

    // 耐久値を減らす処理の関数
    public void TakeDamage(int damage)
    {
        bool canDamage = CanDamage();

        if(canDamage)
        {
            Durability -= damage;
        }

        OnDamaged(damage,canDamage);

        if (Durability <= 0)
        {
            OnBroken();
        }
    }

    protected virtual bool CanDamage() { return true; }

    // ダメージを受けた時の処理の関数
    protected virtual void OnDamaged(int damage,bool canDamage) { }

    // 耐久値が0以下になったときの処理の関数
    protected virtual void OnBroken() { gameObject.SetActive(false); }
}
