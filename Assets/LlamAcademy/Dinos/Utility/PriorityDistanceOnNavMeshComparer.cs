using System;
using System.Collections.Generic;
using LlamAcademy.Dinos.Config;
using LlamAcademy.Dinos.Unit;
using UnityEngine;
using UnityEngine.AI;

namespace LlamAcademy.Dinos.Utility
{
    public struct PriorityDistanceOnNavMeshComparer : IComparer<GameObject>
    {
        private Vector3 Source;
        private NavMeshQueryFilter Filter;
        private AttackConfigSO AttackConfig;

        public PriorityDistanceOnNavMeshComparer(GameObject source, AttackConfigSO attackConfig, NavMeshQueryFilter filter)
        {
            Source = source.transform.position;
            Filter = filter;
            AttackConfig = attackConfig;
        }

        public int Compare(GameObject x, GameObject y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return 1;
            if (y == null) return -1;

            IDamageable xDamageable = x.GetComponent<IDamageable>();
            IDamageable yDamageable = y.GetComponent<IDamageable>();

            int xIndex = Array.FindIndex(AttackConfig.EnemyPriority, item => item == xDamageable.UnitType.Type);
            int yIndex = Array.FindIndex(AttackConfig.EnemyPriority, item => item == yDamageable.UnitType.Type);

            if (xIndex == -1 && yIndex == -1) return 0;
            if (xIndex == -1 && yIndex != -1) return 1;
            if (xIndex != -1 && yIndex == -1) return -1;
            // prefer priority over distance
            if (xIndex < yIndex) return -1;
            if (yIndex < xIndex) return 1;

            // if same priority, then calculate distance
            NavMeshPath path1 = new(), path2 = new();
            // prefer closest point on collider where possible
            Vector3 xPosition = x.transform.position;
            Vector3 yPosition = y.transform.position;
            if (x.TryGetComponent(out Collider xCollider))
            {
                xPosition = xCollider.ClosestPoint(Source);
            }

            if (y.TryGetComponent(out Collider yCollider))
            {
                yPosition = yCollider.ClosestPoint(Source);
            }
            bool path1Success = NavMesh.CalculatePath(Source, xPosition, Filter, path1);
            bool path2Success = NavMesh.CalculatePath(Source, yPosition, Filter, path2);

            float path1Distance = path1Success ? NavMeshUtilities.GetSquareDistanceOfPath(path1) : float.MaxValue;
            float path2Distance = path2Success ? NavMeshUtilities.GetSquareDistanceOfPath(path2) : float.MaxValue;
            return path1Distance.CompareTo(path2Distance);
        }
    }
}
