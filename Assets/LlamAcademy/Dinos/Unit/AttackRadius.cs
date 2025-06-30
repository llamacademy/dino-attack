using UnityEngine;

namespace LlamAcademy.Dinos.Unit
{
    [RequireComponent(typeof(Collider))]
    public class AttackRadius : MonoBehaviour
    {
        public delegate void TargetEvent(IDamageable target);

        public TargetEvent OnTargetEnter;
        public TargetEvent OnTargetExit;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IDamageable damageable))
            {
                OnTargetEnter?.Invoke(damageable);
                damageable.OnDeath += Damageable_OnDeath;
            }
        }

        private void Damageable_OnDeath(IDamageable damageable)
        {
            OnTriggerExit(damageable.Transform.GetComponent<Collider>());
            damageable.OnDeath -= Damageable_OnDeath;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out IDamageable damageable))
            {
                OnTargetExit?.Invoke(damageable);
                damageable.OnDeath -= Damageable_OnDeath;
            }
        }
    }
}
