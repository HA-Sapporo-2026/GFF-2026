// Last assigned by 武藤
using GFF2026.InGame;
using UnityEngine;

namespace GFF2026.InGame 
{
    public class PlayerStatus : MonoBehaviour, IPlayerStatus
    {

        [SerializeField] int initialAttackPower; // 攻撃力の初期値
        [SerializeField] float initialAttackSpeed; // 攻撃速度の初期値
        [SerializeField] float initialMoveSpeed; // 移動速度の初期値

        public int AttackPower { get; private set; }

        public float AttackSpeed { get; private set; }

        public float MoveSpeed { get; private set; }

        /// <summary>
        /// 攻撃力を加算する処理
        /// <summary>
        /// <param name="power">加算する攻撃力<param>
        public void IncreaseAttackPower(int power)
        {
            AttackPower = Mathf.Min(AttackPower + power, 10000); // 10000は適当な最大攻撃力
        }
        /// <summary>
        /// 攻撃速度を加算する処理
        /// <summary>
        /// <param name="speed">加算する攻撃速度<param>
        public void IncreaseAttackSpeed(float speed)
        {
            AttackSpeed = Mathf.Min(AttackSpeed + speed, 100.0f); // 100.0fは適当な最大攻撃速度
        }

        /// <summary>
        ///  移動速度を加算する処理
        /// <summary>
        /// <param name="speed">加算する移動速度<param>
        public void IncreaseMoveSpeed(float speed)
        {
            MoveSpeed = Mathf.Min(MoveSpeed + speed, 100.0f); // 100.0fは適当な最大移動速度
        }
    }

}

