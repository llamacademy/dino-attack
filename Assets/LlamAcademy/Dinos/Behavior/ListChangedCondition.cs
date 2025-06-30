using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;

namespace LlamAcademy.Dinos.Behavior
{
    [Serializable, Unity.Properties.GeneratePropertyBag]
    [Condition(name: "List Changed", story: "[List] changed.", category: "Variable Conditions",
        id: "c64af628fcb18fea7e85db5e7e3d7034")]
    public partial class ListChangedCondition : Condition
    {
        [SerializeReference] public BlackboardVariable<List<GameObject>> List;

        private int LastCount;

        public override bool IsTrue()
        {
            if (List.Value.Count == LastCount)
            {
                return false;
            }

            LastCount = List.Value.Count;
            return true;
        }

        public override void OnStart()
        {
            if (List.Value == null) return;

            LastCount = List.Value.Count;
        }
    }
}
