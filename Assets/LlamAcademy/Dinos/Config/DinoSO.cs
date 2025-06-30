using LlamAcademy.Dinos.Enemy;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LlamAcademy.Dinos.Config
{
    [CreateAssetMenu(fileName = "Dino", menuName = "AI/Dino Unit", order = 0)]
    public class DinoSO : UnitSO
    {
        [field: SerializeField] public Sprite Sprite { get; private set; }
        [field: SerializeField] public new int Cost { get; private set; }
        [field: SerializeField] public Key Hotkey { get; private set; }
    }
}
