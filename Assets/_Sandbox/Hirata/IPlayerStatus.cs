using UnityEngine;

public interface IPlayerStatus 
{
    /// <summary>
    /// 攻撃力
    /// </summary>
    int AttackPower {  get; }

    /// <summary>
    /// 攻撃速度
    /// </summary>
    float AttackSpeed {  get; }

    /// <summary>
    /// 移動速度
    /// </summary>
    float MoveSpeed { get; }
}
