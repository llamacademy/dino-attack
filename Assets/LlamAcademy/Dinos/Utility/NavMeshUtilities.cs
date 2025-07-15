using UnityEngine;
using UnityEngine.AI;

namespace LlamAcademy.Dinos.Utility
{
    public static class NavMeshUtilities
    {
        public static float GetSquareDistanceOfPath(NavMeshPath path)
        {
            Vector3[] corners = path.corners;
            float distance = 0;
            for (int i = 1; i < corners.Length; i++)
            {
                distance += (corners[i - 1] - corners[i]).sqrMagnitude;
            }

            return distance;
        }

        public static bool IsAtTargetLocation(NavMeshAgent agent) =>
            (!agent.hasPath && !agent.pathPending)
                || (agent.hasPath && agent.remainingDistance <= agent.stoppingDistance + agent.radius);
    }
}
