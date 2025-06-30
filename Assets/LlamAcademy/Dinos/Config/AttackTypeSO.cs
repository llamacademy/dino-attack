using System.Linq;
using LlamAcademy.Dinos.Unit;
using UnityEngine;

namespace LlamAcademy.Dinos.Config
{
    public abstract class AttackTypeSO : ScriptableObject
    {
        [field: SerializeField][Range(0.1f, 3)] public float AttackDelay { get; private set; } = 1;
        [field: SerializeField] public bool DelayDamage { get; private set; } = false;
        [field: SerializeField] public float DelayDamageTime { get; private set; } = 0.75f;
        [field: SerializeField] public int Priority { get; private set; }
        [field: SerializeField] public ArmorTypeDamage[] DamagePerType { get; private set; }
        [field: SerializeField] public float MaxAttackRange { get; private set; } = 1.5f;
        [field: SerializeField] public ParticleSystem AttackParticlePrefab { get; private set; }
        [field: SerializeField] public MoveToTargetConfig AttackParticleMoveConfig { get; private set; }

        public int GetDamage(ArmorTypeSO armorType)
        {
            int damage = 0;
            ArmorTypeDamage armorTypeDamage = DamagePerType.FirstOrDefault((type) => type.ArmorType == armorType);

            if (armorTypeDamage != null)
            {
                return armorTypeDamage.Damage;
            }

            Debug.LogWarning($"{name} is missing an Armor Type Damage Config for {armorType.name}. Using 0 damage");
            return damage;

        }

        public abstract void ApplyEffect(IDamageable target);

        [System.Serializable]
        public class ArmorTypeDamage
        {
            public int Damage = 5;
            public ArmorTypeSO ArmorType;
        }

        [System.Serializable]
        public class MoveToTargetConfig
        {
            [field: SerializeField] public bool MoveToTarget { get; private set; }
            [field: SerializeField] public float MoveToTargetTime { get; private set; } = 2f;
            [field: SerializeField] public float MoveToTargetDelay;
            [field: SerializeField] public Vector3 LocalSpawnOffset { get; private set; } = Vector3.zero;
        }
    }
}
