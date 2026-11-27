// Last assigned by 武藤
using GFF2026.InGame;
using Unity.Cinemachine;
using UnityEngine;

namespace GFF2026.InGame 
{
    public class NormalBlockParts : BlockPartsBase
    {
        [SerializeField] CinemachineImpulseSource source;
        protected override void OnDamaged(int damage)
        {
            source.GenerateImpulse();
        }
    }
}

