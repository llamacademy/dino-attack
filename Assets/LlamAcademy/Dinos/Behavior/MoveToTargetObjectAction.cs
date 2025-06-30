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
    [NodeDescription(name: "Move to Target Object", story: "[Self] moves to [TargetObject] .",
        category: "Action/Navigation", id: "7e4a7191d619b55f173bc5d271088a19")]
    public partial class MoveToTargetObjectAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Self;
        [SerializeReference] public BlackboardVariable<GameObject> TargetObject;

        private Vector3 TargetLocation;
        private NavMeshAgent Agent;
        private Collider TargetCollider;

        protected override Status OnStart()
        {
            if (Self.Value == null || !Self.Value.TryGetComponent(out Agent) || TargetObject.Value == null)
                return Status.Failure;

            TargetCollider = TargetObject.Value.GetComponentInChildren<Collider>();
            TargetLocation = GetTargetLocation();

            Agent.SetDestinationImmediate(TargetLocation, Agent.radius + Agent.stoppingDistance + Agent.height);
            return Status.Running;
        }

        private Vector3 GetTargetLocation()
        {
            Vector3 targetLocation;
            if (TargetCollider != null)
            {
                targetLocation = TargetCollider.ClosestPoint(Self.Value.transform.position);
            }
            else
            {
                targetLocation = TargetObject.Value.transform.position;
            }

            if (NavMesh.SamplePosition(targetLocation, out NavMeshHit hit, Agent.radius,
                    new NavMeshQueryFilter() { agentTypeID = Agent.agentTypeID, areaMask = Agent.areaMask }))
            {
                targetLocation = hit.position - (hit.position - targetLocation).normalized * Agent.radius;
            }

            return targetLocation;
        }

        protected override Status OnUpdate()
        {
            Vector3 newTargetLocation = GetTargetLocation();
            if (Vector3.Distance(newTargetLocation, TargetLocation) >= Agent.stoppingDistance)
            {
                Agent.SetDestinationImmediate(newTargetLocation, Agent.radius + Agent.stoppingDistance + Agent.height);
                TargetLocation = newTargetLocation;
                return Status.Running;
            }

            return NavMeshUtilities.IsAtTargetLocation(Agent) ? Status.Success : Status.Running;
        }
    }
}
