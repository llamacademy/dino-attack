using System;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace LlamAcademy.Dinos.RoundManagement
{
    [DefaultExecutionOrder(10)]
    public class NavMeshManager : MonoBehaviour
    {
        [SerializeField]
        private NavMeshSurface EnemySurface;
        [SerializeField]
        private NavMeshSurface[] DinoSurfaces;

        public NavMeshTriangulation EnemyTriangulation;

        public delegate void NavMeshUpdatedEvent(RebakeType type);
        public event NavMeshUpdatedEvent OnNavMeshUpdated;

        public static NavMeshManager Instance
        {
            get;
            private set;
        }

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError($"Multiple NavMeshManagers in the scene. Destroying second one {name}.");
                Destroy(gameObject);
            }
            Instance = this;

            NavMesh.RemoveAllNavMeshData();
            EnemySurface.BuildNavMesh();
            EnemyTriangulation = NavMesh.CalculateTriangulation();

            foreach (NavMeshSurface surface in DinoSurfaces)
            {
                surface.BuildNavMesh();
            }
        }

        public void UpdateNavMesh(RebakeType type, bool updateTriangulation = false)
        {
            switch (type)
            {
                case RebakeType.All:
                {
                    EnemySurface.BuildNavMesh();
                    OnNavMeshUpdated?.Invoke(RebakeType.Enemy);
                    foreach (NavMeshSurface surface in DinoSurfaces)
                    {
                        surface.BuildNavMesh();
                    }
                    OnNavMeshUpdated?.Invoke(RebakeType.Dinos);
                    break;
                }
                case RebakeType.Enemy:
                    // triangulation unfortunately includes ALL Surfaces, so we have a really slow rebake process.
                    if (updateTriangulation)
                    {
                        NavMesh.RemoveAllNavMeshData();
                    }
                    EnemySurface.BuildNavMesh();
                    if (updateTriangulation)
                    {
                        EnemyTriangulation = NavMesh.CalculateTriangulation();
                        UpdateNavMesh(RebakeType.Dinos);
                    }
                    OnNavMeshUpdated?.Invoke(RebakeType.Enemy);
                    break;
                case RebakeType.Dinos:
                default:
                {
                    foreach(NavMeshSurface surface in DinoSurfaces)
                    {
                        surface.BuildNavMesh();
                    }
                    OnNavMeshUpdated?.Invoke(RebakeType.Dinos);
                    break;
                }
            }
        }

        public enum RebakeType
        {
            Enemy,
            Dinos,
            All
        }
    }
}
