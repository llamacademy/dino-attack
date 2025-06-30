using System;
using System.Collections.Generic;
using LlamAcademy.Dinos.Utility;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace LlamAcademy.Dinos.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Sort By Closest On XZ Plane", story: "Sort [GameObjectList] by closest to [Self] on the XZ plane.", category: "Action", id: "4fc28fa21297aae880a52cb436498428")]
    public partial class SortByClosestOnXzPlaneAction : Action
    {
        [SerializeReference] public BlackboardVariable<List<GameObject>> GameObjectList;
        [SerializeReference] public BlackboardVariable<GameObject> Self;
        [SerializeReference] public BlackboardVariable<GameObject> ClosestTarget;

        protected override Status OnStart()
        {
            if (GameObjectList.Value == null || GameObjectList.Value.Count == 0 || Self.Value == null || (GameObjectList.Value.RemoveAll((item) => item == null) > 0 && GameObjectList.Value.Count == 0))
                return Status.Failure;

            GameObjectList.Value.Sort(new DistanceComparer(Self.Value, true));

            ClosestTarget.Value = GameObjectList.Value[0];

            return Status.Success;
        }
    }
}
