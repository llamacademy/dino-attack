using System;
using System.Collections.Generic;
using System.Linq;
using LlamAcademy.Dinos.UI;
using LlamAcademy.Dinos.Unit;
using UnityEngine;

namespace LlamAcademy.Dinos.Utility
{
    [RequireComponent(typeof(Canvas))]
    public class HealthBarCanvas : MonoBehaviour
    {
        public static HealthBarCanvas Instance { get; private set; }

        private Dictionary<Unit.Unit, HealthBar> HealthBars = new();

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError($"Multiple Health Bar Canvases in scene! Destroying this one ({name})!");
                Destroy(this);
                return;
            }

            Instance = this;
        }

        private void Update()
        {
            bool missedCleaningAUnit = false;
            foreach (KeyValuePair<Unit.Unit, HealthBar> keyValuePair in HealthBars)
            {
                if (keyValuePair.Key == null)
                {
                    Debug.LogWarning("Missed cleaning up a unit on death!");
                    missedCleaningAUnit = true;
                }
                else
                {
                    keyValuePair.Value.transform.position = keyValuePair.Key.Transform.position + keyValuePair.Value.FollowOffset;
                }
            }

            if (missedCleaningAUnit)
            {
                IEnumerable<KeyValuePair<Unit.Unit, HealthBar>> keyValuePairs = HealthBars.Where((kvp) => kvp.Key == null).ToArray();
                for (int i = keyValuePairs.Count() - 1; i >= 0; i--)
                {
                    if (keyValuePairs.ElementAt(i).Value != null)
                    {
                        Destroy(keyValuePairs.ElementAt(i).Value.gameObject);
                    }

                    HealthBars.Remove(keyValuePairs.ElementAt(i).Key);
                }
            }
        }

        public void Register(HealthBar healthBar, Unit.Unit unit)
        {
            HealthBars.Add(unit, healthBar);
            healthBar.transform.SetParent(transform);
            healthBar.transform.localRotation = Quaternion.identity;
            unit.OnDeath += HandleUnitDeath;
        }

        private void HandleUnitDeath(IDamageable diedobject)
        {
            Unit.Unit unit = diedobject.Transform.GetComponent<Unit.Unit>();
            if (HealthBars.Remove(unit, out HealthBar healthBar))
            {
                if (healthBar.OnDeathBehavior == HealthBar.DeathBehavior.Destroy)
                {
                    Destroy(healthBar.gameObject);
                }
                else if (healthBar.OnDeathBehavior == HealthBar.DeathBehavior.Disable)
                {
                    healthBar.gameObject.SetActive(false);
                }
            }
        }
    }
}
