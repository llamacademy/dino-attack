using System.Collections.Generic;
using LlamAcademy.Dinos.Behavior;
using LlamAcademy.Dinos.RoundManagement;
using LlamAcademy.Dinos.Unit;
using LlamAcademy.Dinos.Utility;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

namespace LlamAcademy.Dinos.Enemy
{
    [RequireComponent(typeof(NavMeshAgent), typeof(Animator), typeof(BehaviorGraphAgent))]
    [RequireComponent(typeof(Rigidbody))]
    public class Defender : Unit.Unit
    {
        public AIState State =>
            GraphAgent.GetVariable(EnemyGraphConstants.COMMAND, out BlackboardVariable<AIState> stateVariable)
                ? stateVariable.Value
                : AIState.Idle;

        public Vector3 TargetLocation => Agent.enabled && Agent.isOnNavMesh ? Agent.destination : transform.position;
        private IDamageable Target;

        protected override void Start()
        {
            base.Start();
            if (GraphAgent.GetVariable(EnemyGraphConstants.ATTACK_EVENT_CHANNEL, out BlackboardVariable<AttackEventChannel> attackEventChannel))
            {
                // attackEventChannel.Value.Event += HandleAttackEvent;
            }

            NavMeshManager.Instance.OnNavMeshUpdated += OnNavMeshUpdated;
        }

        private void OnDisable()
        {
            NavMeshManager.Instance.OnNavMeshUpdated -= OnNavMeshUpdated;
        }

        private void OnNavMeshUpdated(NavMeshManager.RebakeType type)
        {
            if (type == NavMeshManager.RebakeType.Enemy
                && !NavMesh.SamplePosition(transform.position, out NavMeshHit _, Agent.radius,
                    new NavMeshQueryFilter() { agentTypeID = Agent.agentTypeID, areaMask = Agent.areaMask }))
            {
                // building has been destroyed
                GraphAgent.enabled = false;
                Agent.enabled = false;
                if (Rigidbody != null)
                {
                    Rigidbody.isKinematic = false;
                    Rigidbody.useGravity = true;
                }

                RaiseDeathEvent();
                Die();
            }
        }

        protected override void OnTargetExit(IDamageable target)
        {
            if (GraphAgent.GetVariable(EnemyGraphConstants.NEARBY_ATTACKABLES,
                    out BlackboardVariable<List<GameObject>> nearbyAttackables))
            {
                nearbyAttackables.Value.Remove(target.Transform.gameObject);
                GraphAgent.SetVariableValue(EnemyGraphConstants.NEARBY_ATTACKABLES, nearbyAttackables.Value);

                // maybe need to go back to patrolling, or idling depending on the unit type?
            }
        }

        protected override void OnTargetEnter(IDamageable target)
        {
            if (GraphAgent.GetVariable(EnemyGraphConstants.NEARBY_ATTACKABLES,
                    out BlackboardVariable<List<GameObject>> nearbyAttackables))
            {
                nearbyAttackables.Value.Add(target.Transform.gameObject);
                GraphAgent.SetVariableValue(EnemyGraphConstants.NEARBY_ATTACKABLES, nearbyAttackables.Value);
            }

            GraphAgent.SetVariableValue(EnemyGraphConstants.COMMAND, AIState.Attacking);
        }

        public void SetDestination(Vector3 position)
        {
            GraphAgent.SetVariableValue(EnemyGraphConstants.TARGET_LOCATION, position);
            GraphAgent.SetVariableValue(EnemyGraphConstants.COMMAND, AIState.CommandedMove);
        }

        public void Patrol(List<Vector3> waypoints)
        {
            GraphAgent.SetVariableValue(EnemyGraphConstants.PATROL_WAYPOINTS, waypoints);
            GraphAgent.SetVariableValue(EnemyGraphConstants.COMMAND, AIState.Patrol);
        }

        public void Idle()
        {
            GraphAgent.SetVariableValue(EnemyGraphConstants.COMMAND, AIState.Idle);
        }

        public void Attack(IDamageable damageable)
        {
            GraphAgent.SetVariableValue(EnemyGraphConstants.TARGET, damageable);
            GraphAgent.SetVariableValue(EnemyGraphConstants.COMMAND, AIState.Attacking);
        }

        public override void Die()
        {
            GraphAgent.SetVariableValue(EnemyGraphConstants.COMMAND, AIState.Idle);
            Animator.SetTrigger(AnimationConstants.DIE_PARAMETER);
            if (TryGetComponent(out Collider collider))
            {
                collider.enabled = false;
            }
            Invoke(nameof(DestroyGO), 2.5f);
        }

        private void DestroyGO()
        {
            RaiseDeathEvent(); // final raise death event because other ones...don't always remove the object and a
                               // destroyed objects gets stuck in the known objects list
            Destroy(gameObject);
        }
    }
}
