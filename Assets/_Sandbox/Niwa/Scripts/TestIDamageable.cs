using GFF2026.InGame;
using UnityEngine;

namespace Sandbox.Niwa
{
    public class TestIDamageable : MonoBehaviour, IDamageable
    {
        public void TakeDamage(int damage)
        {
            Debug.Log(damage);
        }
    }
}
