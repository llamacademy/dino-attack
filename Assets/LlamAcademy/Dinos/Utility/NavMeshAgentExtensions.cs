using UnityEngine;
using UnityEngine.AI;

namespace LlamAcademy.Dinos.Utility
{
    public static class NavMeshAgentExtensions
    {
        public static bool SetDestinationImmediate(
            this NavMeshAgent agent,
            Vector3 targetLocation,
            float positionLeniency = 0)
        {
            NavMeshPath path = new();
            NavMeshQueryFilter queryFilter = new() {
                agentTypeID = agent.agentTypeID,
                areaMask = agent.areaMask
            };
            if (positionLeniency != 0)
            {
                if (!NavMesh.SamplePosition(targetLocation, out NavMeshHit hit, positionLeniency, queryFilter))
                {
                    return false;
                }

                targetLocation = hit.position;
            }

            bool canSetPath = NavMesh.CalculatePath(
                agent.transform.position,
                targetLocation,
                queryFilter,
                path
            );

            if (canSetPath)
            {
                agent.SetPath(path);
            }

            return canSetPath;
        }
    }
}
