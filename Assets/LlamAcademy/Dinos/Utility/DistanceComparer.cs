using System.Collections.Generic;
using UnityEngine;

namespace LlamAcademy.Dinos.Utility
{
    public struct DistanceComparer : IComparer<GameObject>
    {
        private Vector3 Source;
        private bool IgnoreY;

        public DistanceComparer(GameObject source, bool ignoreY = false)
        {
            Source = source.transform.position;
            IgnoreY = ignoreY;
        }

        public int Compare(GameObject x, GameObject y)
        {
            if (x != null && y == null) return 1;
            if (x == null && y != null) return -1;
            if (x == null && y == null) return 0;

            if (IgnoreY)
            {
                Vector3 xPosition = x.transform.position;
                xPosition = new Vector3(xPosition.x, Source.y, xPosition.z);

                Vector3 yPosition = y.transform.position;
                yPosition = new Vector3(yPosition.x, Source.y, yPosition.z);

                return (xPosition - Source).sqrMagnitude.CompareTo((yPosition - Source).sqrMagnitude);
            }

            return (x.transform.position - Source).sqrMagnitude.CompareTo((y.transform.position - Source).sqrMagnitude);
        }
    }
}
