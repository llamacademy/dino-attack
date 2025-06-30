using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LlamAcademy.Dinos.Config;
using LlamAcademy.Dinos.RoundManagement;
using UnityEngine;
using UnityEngine.AI;

namespace LlamAcademy.Dinos.Player
{
    public class PlaceDinoVisualization : MonoBehaviour
    {
        public bool IsValidPlacementLocation { get; private set; }

        [SerializeField] private float SafeSpawnDistance = 15f;
        [SerializeField] private List<GameObject> Visualizations = new();
        [SerializeField] private LayerMask UnsafeLayers;

        [SerializeField] private SafeZoneVisualizer SafeZonePrefab;
        [SerializeField] private GameObject WorldRoot;

        private List<Renderer> Renderers = new();
        private Collider[] Hits = new Collider[1];
        private int Count = 1;
        private DinoSO Dino;
        [SerializeField] private GameObject SafeZone;

        private static readonly int TINT = Shader.PropertyToID("_Tint");
        private static readonly int FRESNEL_COLOR = Shader.PropertyToID("_FresnelColor");

        private void Start()
        {
            // If you're not using a shader to show this, this code works nicely :)
            // List<Collider> colliders = WorldRoot.GetComponentsInChildren<Collider>().Where(collider => collider.enabled && !collider.isTrigger && (UnsafeLayers | 1 << collider.gameObject.layer) == UnsafeLayers).ToList();
            // SafeZone = new  GameObject("Safe Zone");
            // foreach (Collider collider in colliders)
            // {
            //     float radius = (collider.bounds.extents.x + collider.bounds.extents.z) / 2f + SafeSpawnDistance;
            //     SafeZoneVisualizer safeZone = Instantiate(SafeZonePrefab, collider.transform.position, Quaternion.identity, SafeZone.transform);
            //     safeZone.name = $"Safe Zone for {collider.name}";
            //     safeZone.transform.localScale = new Vector3(radius, radius, radius);
            //     safeZone.transform.position = collider.transform.position;
            //     // Assume default layer is unsafe, so good enough.
            // }

            RoundManager.Instance.OnGameStateChange += OnGameStateChange;
        }

        private void OnDestroy()
        {
            RoundManager.Instance.OnGameStateChange -= OnGameStateChange;
        }

        private void OnGameStateChange(GameState oldState, GameState newState)
        {
            if (newState == GameState.Running)
            {
                StartCoroutine(FadeOut(SafeZone.GetComponent<Renderer>().material));
            }
        }

        private IEnumerator FadeIn(Material material)
        {
            SafeZone.SetActive(true);
            float time = 0;
            int STRENGTH_PROPERTY = Shader.PropertyToID("_Strength");
            while (time < 1)
            {
                material.SetFloat(STRENGTH_PROPERTY, time);
                time += Time.deltaTime * 4;
                yield return null;
            }

            material.SetFloat(STRENGTH_PROPERTY, 1);
        }

        private IEnumerator FadeOut(Material material)
        {
            float time = 1;
            int STRENGTH_PROPERTY = Shader.PropertyToID("_Strength");
            while (time > 0)
            {
                material.SetFloat(STRENGTH_PROPERTY, time);
                time -= Time.deltaTime * 4;
                yield return null;
            }

            material.SetFloat(STRENGTH_PROPERTY, 0);
            SafeZone.SetActive(false);
        }

        public void ChangeDino(DinoSO dinoSO)
        {
            foreach (GameObject go in Visualizations)
            {
                Destroy(go.gameObject);
            }

            Visualizations.Clear();
            Renderers.Clear();
            Dino = dinoSO;

            if (dinoSO == null)
            {
                StartCoroutine(FadeOut(SafeZone.GetComponent<Renderer>().material));
                return;
            }

            if (!SafeZone.activeSelf)
            {
                StartCoroutine(FadeIn(SafeZone.GetComponent<Renderer>().material));
            }

            for (int i = 0; i < Count; i++)
            {
                Unit.Unit dino = Instantiate(dinoSO.Prefab, transform.position, Quaternion.LookRotation((RoundManager.Instance.DinoTarget.position - transform.position).normalized), transform);
                foreach (Collider collider in dino.GetComponentsInChildren<Collider>())
                {
                    collider.enabled = false;
                }
                dino.GetComponent<NavMeshAgent>().enabled = false;
                dino.transform.localPosition = Vector3.zero;
                Visualizations.Add(dino.gameObject);
                Renderer renderer = dino.GetComponentInChildren<Renderer>();
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                Renderers.Add(renderer);
            }
        }

        private void Update()
        {
            for (int i = 0; i < Visualizations.Count; i++)
            {
                if (Dino != null &&
                    DinoSpawner.Instance.ResourcesToSpend < Dino.Cost
                    || Physics.OverlapSphereNonAlloc(
                        Visualizations[i].transform.position,
                        0.25f, // small leniency since on Start we handle spawning objects to block
                        Hits,
                        UnsafeLayers) > 0)
                {
                    IsValidPlacementLocation = false;
                    Renderers[i].material.SetColor(TINT, Color.red);
                    Renderers[i].material.SetColor(FRESNEL_COLOR, Color.red);
                }
                else
                {
                    IsValidPlacementLocation = true;
                    Renderers[i].material.SetColor(TINT, Color.cyan);
                    Renderers[i].material.SetColor(FRESNEL_COLOR, Color.cyan);
                }
            }
        }
    }
}
