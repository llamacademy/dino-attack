using System;
using LlamAcademy.Dinos.RoundManagement;
using LlamAcademy.Dinos.Utility;
using UnityEngine;

namespace LlamAcademy.Dinos.Unit
{
    public class Wall : Unit
    {
        [SerializeField] private GameObject Root;
        [field: SerializeField] public float HealthToCostMultiplier { get; private set; } = 0.2f;

        protected override void Awake()
        {
            base.Awake();

            if (Root == null)
            {
                Root = gameObject;
            }

            Rigidbody = null;
        }

        private void OnDestroy()
        {
            if (HealthBar != null)
            {
                Destroy(HealthBar.gameObject);
            }
        }

        private void OnEnable()
        {
            if (HealthBar != null)
            {
                HealthBar.gameObject.SetActive(true);
                HealthBarCanvas.Instance.Register(HealthBar, this);
            }
        }

        protected override void OnTargetEnter(IDamageable target) {}

        protected override void OnTargetExit(IDamageable target) {}

        public void Repair(int amount)
        {
            if (Health == 0 && amount > 0)
            {
                gameObject.SetActive(true);
            }
            Health += amount;
            if (Health > MaxHealth)
            {
                Debug.LogWarning($"Health attempted to exceed max health by {MaxHealth - Health}. Clamped to Max Health. This probably means there is a math error elsewhere.");
                Health = MaxHealth;
            }

            if (HealthBar != null)
            {
                HealthBar.SetProgress((float)Health / MaxHealth);
            }
        }

        public override void Die()
        {
            // destruction here would be cool
            Root.gameObject.SetActive(false);
            NavMeshManager.Instance.UpdateNavMesh(NavMeshManager.RebakeType.All);
        }
    }
}
