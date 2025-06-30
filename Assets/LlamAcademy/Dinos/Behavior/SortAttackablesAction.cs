using System;
using System.Collections.Generic;
using LlamAcademy.Dinos.Config;
using LlamAcademy.Dinos.Utility;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;

namespace LlamAcademy.Dinos.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Sort Attackables", story: "Sort [NearbyAttackables] by closest to [Self] following [AttackConfig] priority.", category: "Action", id: "36069df22dc1b1da3032feed5ea3865d")]
    public partial class SortAttackablesAction : Action
    {
        [SerializeReference] public BlackboardVariable<List<GameObject>> NearbyAttackables;
        [SerializeReference] public BlackboardVariable<GameObject> Self;
        [SerializeReference] public BlackboardVariable<AttackConfigSO> AttackConfig;
        [SerializeReference] public BlackboardVariable<GameObject> ClosestAttackable;

        protected override Status OnStart()
        {
            if (NearbyAttackables.Value == null || NearbyAttackables.Value.Count == 0 || Self.Value == null || !Self.Value.TryGetComponent(out NavMeshAgent agent))
                return Status.Failure;

            List<GameObject> attackables = NearbyAttackables.Value;
            attackables.Sort(new PriorityDistanceOnNavMeshComparer(Self.Value, AttackConfig.Value, new NavMeshQueryFilter()
            {
                agentTypeID = agent.agentTypeID,
                areaMask = agent.areaMask
            }));

            NearbyAttackables.Value = attackables;
            ClosestAttackable.Value = attackables[0];

            return Status.Success;
        }
    }
}
