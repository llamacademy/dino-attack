using LlamAcademy.Dinos.Enemy;
using UnityEngine;

namespace LlamAcademy.Dinos.Unit
{
    public interface IDamageable
    {
        public int MaxHealth { get; set; }
        public int Health { get; set; }

        public Transform Transform { get; }
        public UnitSO UnitType { get; }

        public delegate void TakeDamageEvent(IDamageable damageable, int damage);
        public event TakeDamageEvent OnTakeDamage;

        public delegate void DeathEvent(IDamageable diedObject);
        public event DeathEvent OnDeath;

        public void TakeDamage(int damage);
    }
}
