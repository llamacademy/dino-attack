using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace LlamAcademy.Dinos.Utility
{
    public struct DistanceOnNavMeshComparer : IComparer<GameObject>
    {
        private Vector3 Source;
        private NavMeshQueryFilter Filter;
        public DistanceOnNavMeshComparer(GameObject source, NavMeshQueryFilter filter)
        {
            Source = source.transform.position;
            Filter = filter;
        }

        public int Compare(GameObject x, GameObject y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return 1;
            if (y == null) return -1;
            NavMeshPath path1 = new(), path2 = new();
            NavMesh.CalculatePath(Source, x.transform.position, Filter, path1);
            NavMesh.CalculatePath(Source, y.transform.position, Filter, path2);

            return path1.corners.Sum(CornerToSquareMagnitude).CompareTo(path2.corners.Sum(CornerToSquareMagnitude));
        }

        private float CornerToSquareMagnitude(Vector3 corner) => corner.sqrMagnitude;
    }
}
