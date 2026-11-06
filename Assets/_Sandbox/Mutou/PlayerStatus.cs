using GFF2026.InGame;
using UnityEngine;

public class PlayerStatus : MonoBehaviour, IPlayerStatus
{
    public int AttackPower { get; private set; }

    public float AttackSpeed {  get; private set; }

    public float MoveSpeed {  get; private set; }

    [SerializeField] int initialAttackPower; // 攻撃力の初期値
    [SerializeField] float initialAttackSpeed; // 攻撃速度の初期値
    [SerializeField] float initialMoveSpeed; // 移動速度の初期値

    public void IncreaseAttackPower(int power) // 攻撃力を加算する処理
    {
        AttackPower = Mathf.Min(AttackPower + power, 10000); // 10000は適当な最大攻撃力
    }

    public void IncreaseAttackSpeed(float speed) // 攻撃速度を加算する処理
    {
        AttackSpeed = Mathf.Min(AttackSpeed + speed, 100.0f); // 100.0fは適当な最大攻撃速度
    }

    public void IncreaseMoveSpeed(float speed) // 移動速度を加算する処理
    {
        MoveSpeed = Mathf.Min(MoveSpeed + speed, 100.0f); // 100.0fは適当な最大移動速度
    }
}
