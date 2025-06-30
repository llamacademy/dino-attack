using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;

namespace LlamAcademy.Dinos.Behavior
{
    [Serializable, Unity.Properties.GeneratePropertyBag]
    [Condition(name: "List Size Comparison", story: "[List] size is [Operator] [Value]", category: "Variable Conditions", id: "cafff8caff188e6834b3d2f891ead100")]
    public partial class ListSizeComparisonCondition : Condition
    {
        [SerializeReference] public BlackboardVariable<List<GameObject>> List;
        [Comparison(comparisonType: ComparisonType.All)]
        [SerializeReference] public BlackboardVariable<ConditionOperator> Operator;
        [SerializeReference] public BlackboardVariable<int> Value;

        public override bool IsTrue()
        {
            if (List.Value == null) return false;

            return Operator.Value switch
            {
                ConditionOperator.Equal => List.Value.Count == Value,
                ConditionOperator.NotEqual => List.Value.Count != Value,
                ConditionOperator.Greater => List.Value.Count > Value,
                ConditionOperator.Lower => List.Value.Count < Value,
                ConditionOperator.GreaterOrEqual => List.Value.Count >= Value,
                ConditionOperator.LowerOrEqual => List.Value.Count <= Value,
                _ => false
            };
        }
    }
}
