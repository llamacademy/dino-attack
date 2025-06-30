using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LlamAcademy.Dinos.Config
{
    [CreateAssetMenu(fileName = "Resource", menuName = "Resource", order = 10)]
    public class ResourceSO : ScriptableObject
    {
        public Sprite Icon;
        public int Amount;

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#else
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
#endif
        public static void RegisterConverters()
        {
            ConverterGroup group = new ("Integer Format");
            group.AddConverter((ref int value) => value.ToString("N0"));

            ConverterGroups.RegisterConverterGroup(group);

            group = new ("Key Sanitizer");
            group.AddConverter((ref Key value) => value.ToString().Replace("Digit", ""));

            ConverterGroups.RegisterConverterGroup(group);
        }
    }
}
