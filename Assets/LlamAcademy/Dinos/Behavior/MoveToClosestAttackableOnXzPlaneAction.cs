using System;
using System.Linq;
using LlamAcademy.Dinos.Config;
using LlamAcademy.Dinos.Unit;
using LlamAcademy.Dinos.Utility;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;

namespace LlamAcademy.Dinos.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Move to Closest Attackable on XZ Plane", story: "[Self] moves to [TargetObject] on XZ Plane.", category: "Action/GameObject", id: "f485374e7293e9f1c75776ff7efd865f")]
    public partial class MoveToClosestAttackableOnXzPlaneAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Self;
        [SerializeReference] public BlackboardVariable<GameObject> TargetObject;
        [SerializeReference] public BlackboardVariable<AttackConfigSO> AttackConfig;

        private Vector3 TargetLocation;
        private AttackTypeSO MinAttackDistanceAttackType;
        private NavMeshAgent Agent;
        private IDamageable TargetDamageable;
        private Collider TargetCollider;

        protected override Status OnStart()
        {
            if (Self.Value == null || !Self.Value.TryGetComponent(out Agent) || TargetObject.Value == null)
                return Status.Failure;

            if (AttackConfig.Value != null)
            {
                MinAttackDistanceAttackType = GetMinAttackDistanceAttackType();
            }

            TargetCollider = TargetObject.Value.GetComponentInChildren<Collider>();
            TargetDamageable = TargetObject.Value.GetComponent<IDamageable>();
            TargetLocation = GetTargetLocation();

            Agent.SetDestinationImmediate(TargetLocation, Agent.radius + Agent.stoppingDistance + Agent.height);
            return Status.Running;
        }

        private AttackTypeSO GetMinAttackDistanceAttackType()
        {
            AttackTypeSO minAttackDistanceAttackType = AttackConfig.Value.AttackTypes.First();
            for (int i = 1; i < AttackConfig.Value.AttackTypes.Length; i++)
            {
                if (MinAttackDistanceAttackType.GetMinAttackRange(TargetDamageable) > AttackConfig.Value.AttackTypes[i].GetMinAttackRange(TargetDamageable))
                {
                    MinAttackDistanceAttackType = AttackConfig.Value.AttackTypes[i];
                }
            }

            return minAttackDistanceAttackType;
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

            targetLocation.y = Self.Value.transform.position.y;

            NavMeshQueryFilter queryFilter = new () { agentTypeID = Agent.agentTypeID, areaMask = Agent.areaMask };
            NavMesh.Raycast(Self.Value.transform.position, targetLocation, out NavMeshHit raycastHit, queryFilter);
            NavMesh.SamplePosition(targetLocation, out NavMeshHit samplePositionHit, Agent.radius + Agent.stoppingDistance, queryFilter);

            if (raycastHit.hit || samplePositionHit.hit)
            {
                NavMeshHit hit = GetClosestHit(raycastHit, samplePositionHit, targetLocation);
                targetLocation = hit.position - (hit.position - Self.Value.transform.position).normalized * Agent.radius;
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

            if (AttackConfig.Value != null)
            {
                if (XZDistanceIsLessThanMaxAttackDistance(MinAttackDistanceAttackType) || NavMeshUtilities.IsAtTargetLocation(Agent))
                {
                    return Status.Success;
                }

                return Status.Running;
            }

            return NavMeshUtilities.IsAtTargetLocation(Agent) ? Status.Success : Status.Running;
        }

        protected override void OnEnd()
        {
            Agent.destination = Agent.transform.position;
        }

        private bool XZDistanceIsLessThanMaxAttackDistance(AttackTypeSO attack)
        {
            Vector3 xzTargetPosition = new (TargetObject.Value.transform.position.x, Agent.transform.position.y, TargetObject.Value.transform.position.z);
            return Vector3.Distance(xzTargetPosition, Agent.transform.position) <= attack.GetMinAttackRange(TargetDamageable);
        }

        private NavMeshHit GetClosestHit(NavMeshHit hit1, NavMeshHit hit2, Vector3 target)
        {
            if (hit1.hit && hit2.hit)
            {
                return Vector3.Distance(hit1.position, target) <= Vector3.Distance(hit2.position, target)
                    ? hit1
                    : hit2;
            }

            if (hit1.hit && !hit2.hit)
            {
                return hit1;
            }

            return hit2;
        }
    }
}
