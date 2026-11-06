using GFF2026.InGame;
using UnityEngine;

namespace Sandbox.Hirata
{
    public class TestPlayerStatus : MonoBehaviour, IPlayerStatus
    {
        [SerializeField] int attackPower;
        [SerializeField] float attackSpeed;
        [SerializeField] float moveSpeed;

        public int AttackPower => attackPower;
        public float AttackSpeed => attackSpeed;
        public float MoveSpeed => moveSpeed;
    }

}