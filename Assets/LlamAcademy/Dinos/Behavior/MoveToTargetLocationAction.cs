using System;
using LlamAcademy.Dinos.Utility;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;

namespace LlamAcademy.Dinos.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Move to Target Location", story: "[Self] moves to [TargetLocation] .",
        category: "Action/Navigation", id: "858c357f788f621d11397052c9f01480")]
    public partial class MoveToTargetLocationAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Self;
        [SerializeReference] public BlackboardVariable<Vector3> TargetLocation;
        [SerializeReference] public BlackboardVariable<bool> IsAbleToSetDestination;

        private NavMeshAgent Agent;

        protected override Status OnStart()
        {
            if (Self.Value == null || !Self.Value.TryGetComponent(out Agent)) return Status.Failure;

            IsAbleToSetDestination.Value = Agent.SetDestinationImmediate(TargetLocation.Value, Agent.radius + Agent.stoppingDistance + Agent.height);
            return !IsAbleToSetDestination ? Status.Success : Status.Running;
        }

        protected override Status OnUpdate()
        {
            return NavMeshUtilities.IsAtTargetLocation(Agent) ? Status.Success : Status.Running;
        }
    }
}
