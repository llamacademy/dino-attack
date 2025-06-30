using LlamAcademy.Dinos.Config;
using LlamAcademy.Dinos.RoundManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace LlamAcademy.Dinos.Player
{
    [DefaultExecutionOrder(10)]
    public class DinoSpawner : MonoBehaviour
    {
        [SerializeField] private int AvailableResources;
        [SerializeField] private ResourceSO FoodResource;
        public int ResourcesToSpend
        {
            get => FoodResource.Amount;
            set => FoodResource.Amount = value;
        }

        public static DinoSpawner Instance { get; private set; }
        public delegate void SpawnDinoEvent(Unit.Unit spawnedDino);
        public event SpawnDinoEvent OnSpawnDino;
        public delegate void DinoDeathEvent(Unit.Unit deadDino);
        public event DinoDeathEvent OnDinoDeath;

        private DinoSO SpawnDino;

        [SerializeField]
        private Camera Camera;
        [SerializeField]
        private LayerMask GroundLayer;

        [SerializeField]
        private PlaceDinoVisualization Visualization;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError($"Multiple RoundManagers detected. Deleting the second one {name}");
                Destroy(gameObject);
                return;
            }
            Instance = this;
            ResourcesToSpend = 0;
        }

        private void Start()
        {
            RoundManager.Instance.OnGameStateChange += Instance_OnGameStateChange;
        }

        private void Instance_OnGameStateChange(GameState oldState, GameState newState)
        {
            if (newState != GameState.Setup)
            {
                Visualization.ChangeDino(null);
            }
            else
            {
                Visualization.ChangeDino(SpawnDino);
            }
        }

        public void Update()
        {
            if (RoundManager.Instance.State == GameState.Setup)
            {
                if (Physics.Raycast(
                           Camera.ScreenPointToRay(Mouse.current.position.ReadValue()),
                           out RaycastHit hit,
                           float.MaxValue,
                           GroundLayer))
                {
                    Visualization.transform.position = hit.point;
                }

                if (Mouse.current.leftButton.wasReleasedThisFrame
                     && SpawnDino != null
                     && HasResourcesToSpawn(SpawnDino)
                     && Visualization.IsValidPlacementLocation
                     && hit.collider != null
                     && !EventSystem.current.IsPointerOverGameObject())
                {
                    ResourcesToSpend -= SpawnDino.Cost;
                    Unit.Unit spawnedDino = Instantiate(SpawnDino.Prefab, hit.point, Quaternion.LookRotation((RoundManager.Instance.DinoTarget.position - hit.point).normalized));
                    spawnedDino.OnDeath += (obj) => OnDinoDeath?.Invoke(obj.Transform.GetComponent<Unit.Unit>());
                    spawnedDino.UnitType = SpawnDino;
                    spawnedDino.enabled = true;
                    OnSpawnDino?.Invoke(spawnedDino);
                    SpawnDino = null;
                    Visualization.ChangeDino(null);
                }

                if (Keyboard.current.escapeKey.wasReleasedThisFrame)
                {
                    SpawnDino = null;
                    Visualization.ChangeDino(null);
                }
            }
        }

        public void ChangeActiveDino(DinoSO Dino)
        {
            SpawnDino = Dino;
            if (RoundManager.Instance.State == GameState.Setup)
            {
                Visualization.ChangeDino(Dino);
            }
        }

        private bool HasResourcesToSpawn(DinoSO Dino)
        {
            return ResourcesToSpend >= Dino.Cost;
        }
    }
}
