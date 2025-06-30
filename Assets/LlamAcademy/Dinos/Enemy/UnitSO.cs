using LlamAcademy.Dinos.Config;
using LlamAcademy.Dinos.Unit;
using UnityEngine;

namespace LlamAcademy.Dinos.Enemy
{
    [CreateAssetMenu(fileName = "Unit", menuName = "AI/Enemy Unit", order = -10)]
    public class UnitSO : ScriptableObject
    {
        [field: SerializeField] public UnitType Type { get; private set; }
        [field: SerializeField] public ArmorTypeSO ArmorType { get; private set; }
        [field: SerializeField] public Unit.Unit Prefab { get; private set; }
        [field: SerializeField] public int Health { get; private set; }
        [field: SerializeField] public float HealthToCostRatio { get; private set; } = 0.2f;
        public int Cost => Mathf.CeilToInt(Health * HealthToCostRatio);
        [field: SerializeField] public AttackConfigSO AttackConfig { get; private set; }

        [field: SerializeField] public UnitSO Upgrade { get; private set; }
    }
}
