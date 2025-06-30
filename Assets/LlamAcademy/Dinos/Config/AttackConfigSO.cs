using System.Linq;
using LlamAcademy.Dinos.Unit;
using UnityEngine;

namespace LlamAcademy.Dinos.Config
{
    [CreateAssetMenu(fileName = "Attack Config", menuName = "AI/Attack Config", order = 1)]
    public class AttackConfigSO : ScriptableObject
    {
        [field: SerializeField] public LayerMask AttackMask { get; private set; }
        [field: SerializeField] public UnitType[] EnemyPriority { get; private set; }= new []
            { UnitType.Archer, UnitType.Building, UnitType.Cannoneer, UnitType.Mage };
        [field: SerializeField] public AttackTypeSO[] AttackTypes { get; private set; }
        [field: SerializeField] public float PostAttackCooldown { get; private set; } = 0.33f;

        public float GetAttackDelay(AttackTypeSO attackType = null)
        {
            AttackTypeSO attackTypeSO = AttackTypes.First(item => item.Equals(attackType));

            return attackTypeSO == null ? AttackTypes[0].AttackDelay : attackTypeSO.AttackDelay;
        }

        public float GetMaxAttackRange(AttackTypeSO attackType = null)
        {
            AttackTypeSO attackTypeSO = AttackTypes.First(item => item.Equals(attackType));

            return attackTypeSO == null ? AttackTypes.Max(type => type.MaxAttackRange) : attackTypeSO.MaxAttackRange;
        }

        public float GetMinAttackRange() => AttackTypes.Min(type => type.MaxAttackRange);

        public float GetDamage(ArmorTypeSO thisArmorType, AttackTypeSO attackType = null)
        {
            AttackTypeSO attackTypeSO = AttackTypes.First(item => item.Equals(attackType));

            return attackTypeSO == null ? AttackTypes[0].GetDamage(thisArmorType) : attackTypeSO.GetDamage(thisArmorType);
        }
    }
}
