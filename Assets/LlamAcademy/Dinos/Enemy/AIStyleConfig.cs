using System.Collections.Generic;
using UnityEngine;

namespace LlamAcademy.Dinos.Enemy
{
    [CreateAssetMenu(fileName = "AI Config", menuName = "AI/AI Config", order = -9)]
    public class AIStyleConfig : ScriptableObject
    {
        [field: SerializeField] public AISpawnStyle SpawnStyle { get; private set; }
        [field: SerializeField] public Vector2 DefenseSpendRange { get; private set; } = Vector2.one;
        [field: SerializeField] public Vector2 OffenseSpendRange { get; private set; } = Vector2.one;
        [field: SerializeField] public List<WeightedUnitSO> Units { get; private set; }= new();

        [System.Serializable]
        public class WeightedUnitSO
        {
            public UnitSO UnitSO;
            public Vector2 WeightRange = new(0.5f, 1);
        }
    }
}
