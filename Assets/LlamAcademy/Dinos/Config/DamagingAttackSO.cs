using LlamAcademy.Dinos.Unit;
using UnityEngine;

namespace LlamAcademy.Dinos.Config
{
    [CreateAssetMenu(fileName = "Auto Attack", menuName = "AI/Attacks/Auto Attack")]
    public class DamagingAttackSO : AttackTypeSO
    {
        public override void ApplyEffect(IDamageable target)
        {
            target.TakeDamage(GetDamage(target.UnitType.ArmorType));
        }
    }
}
