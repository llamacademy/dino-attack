using UnityEngine;

namespace LlamAcademy.Dinos.Player
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class SafeZoneVisualizer : MonoBehaviour
    {
        [field: SerializeField] public MeshRenderer Renderer { get; private set; }
        [field: SerializeField] public MeshFilter Filter { get; private set; }

        private void Awake()
        {
            if (Renderer == null)
            {
                Renderer = GetComponent<MeshRenderer>();
            }

            if (Filter == null)
            {
                Filter = GetComponent<MeshFilter>();
            }
        }
    }
}
