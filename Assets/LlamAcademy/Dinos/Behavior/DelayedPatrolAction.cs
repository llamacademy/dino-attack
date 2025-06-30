using System;
using System.Collections.Generic;
using System.Linq;
using LlamAcademy.Dinos.Utility;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Action = Unity.Behavior.Action;
using Random = UnityEngine.Random;

namespace LlamAcademy.Dinos.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Delayed Patrol",
        story: "[Self] patrols between [Waypoints] , roaming briefly between each stop.", category: "Action/Navigation",
        id: "650bf3c61b07a4c8ad7f2df73a85dae9")]
    public partial class DelayedPatrolAction : Action
    {
        [FormerlySerializedAs("Agent")] [SerializeReference]
        public BlackboardVariable<GameObject> Self;

        [SerializeReference] public BlackboardVariable<List<Vector3>> Waypoints;
        [SerializeReference] public BlackboardVariable<Vector2> WaypointRoamTime = new(new Vector2(3, 6));

        private NavMeshAgent NavMeshAgent;
        private Animator Animator;

        private List<Vector3> NavMeshWaypoints;
        private Vector3 CurrentTarget;
        private int CurrentPatrolPoint;
        private bool IsWaiting;
        private float WaitingTimer;

        protected override Status OnStart()
        {
            if (Self.Value == null || !Self.Value.TryGetComponent(out NavMeshAgent) || Waypoints.Value == null ||
                Waypoints.Value.Count == 0)
            {
                return Status.Failure;
            }

            Animator = Self.Value.GetComponentInChildren<Animator>();

            IsWaiting = false;
            WaitingTimer = 0.0f;

            PickNavMeshWaypoints();
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (Self.Value == null || Waypoints.Value == null)
            {
                return Status.Failure;
            }

            if (IsWaiting)
            {
                if (WaitingTimer > 0.0f)
                {
                    WaitingTimer -= Time.deltaTime;
                }
                else
                {
                    WaitingTimer = 0f;
                    IsWaiting = false;
                    MoveToNextWaypoint();
                }
            }
            else
            {
                float distance = NavMeshAgent.remainingDistance;


                if (distance <= NavMeshAgent.stoppingDistance + NavMeshAgent.radius)
                {
                    WaitingTimer = Random.Range(WaypointRoamTime.Value.x, WaypointRoamTime.Value.y);
                    IsWaiting = true;
                }
            }

            return Status.Running;
        }

        protected override void OnEnd()
        {
            if (NavMeshAgent.isOnNavMesh)
            {
                NavMeshAgent.ResetPath();
            }
        }

        private void PickNavMeshWaypoints()
        {
            NavMeshQueryFilter queryFilter = new()
                { agentTypeID = NavMeshAgent.agentTypeID, areaMask = NavMeshAgent.areaMask };
            NavMeshWaypoints = new List<Vector3>(Waypoints.Value.Count);
            int closestWaypointIndex = 0;
            int index = 0;
            float closestWaypointDistance = float.MaxValue;
            Vector3 currentPosition = NavMeshAgent.transform.position;

            foreach (Vector3 waypoint in Waypoints.Value)
            {
                if (NavMesh.SamplePosition(waypoint, out NavMeshHit hit, NavMeshAgent.radius, queryFilter))
                {
                    NavMeshPath path = new();
                    if (NavMesh.CalculatePath(currentPosition, hit.position, queryFilter, path))
                    {
                        NavMeshWaypoints.Add(hit.position);
                        float distance = NavMeshUtilities.GetSquareDistanceOfPath(path);

                        if (distance < closestWaypointDistance)
                        {
                            closestWaypointIndex = index;
                            closestWaypointDistance = distance;
                        }

                        index++;
                    }
                }
                else
                {
                    Debug.LogWarning(
                        $"Waypoint at {waypoint} is not close enough to the NavMesh to use as a waypoint!");
                }
            }

            if (NavMeshWaypoints.Count == 0) return;

            CurrentPatrolPoint = closestWaypointIndex;
            CurrentTarget = NavMeshWaypoints[CurrentPatrolPoint];
            NavMeshAgent.SetDestination(CurrentTarget);
        }

        private void MoveToNextWaypoint()
        {
            CurrentPatrolPoint = (CurrentPatrolPoint + 1) % NavMeshWaypoints.Count;

            CurrentTarget = NavMeshWaypoints[CurrentPatrolPoint];
            NavMeshAgent.SetDestination(CurrentTarget);
        }
    }
}
