using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;

namespace LlamAcademy.Dinos.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Stop NavMeshagent", story: "[Self] stops moving.", category: "Action/Navigation",
        id: "7a82aca74cff9fd9f9ee7e6bd1828fdc")]
    public partial class StopNavMeshagentAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Self;

        protected override Status OnStart()
        {
            if (Self.Value == null || !Self.Value.TryGetComponent(out NavMeshAgent agent)) return Status.Failure;

            if (agent.enabled && agent.isOnNavMesh && agent.hasPath)
            {
                agent.ResetPath();
            }
            return Status.Success;
        }
    }
}
