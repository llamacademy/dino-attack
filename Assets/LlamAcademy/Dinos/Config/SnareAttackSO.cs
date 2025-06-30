using System.Collections;
using LlamAcademy.Dinos.Unit;
using UnityEngine;
using UnityEngine.AI;

namespace LlamAcademy.Dinos.Config
{
    [CreateAssetMenu(fileName = "Auto Attack", menuName = "AI/Attacks/Snaring Attack")]
    public class SnareAttackSO : DamagingAttackSO
    {
        [field: SerializeField] public float SnareDuration { get; private set; } = 5f;

        public override void ApplyEffect(IDamageable damageable)
        {
            base.ApplyEffect(damageable);

            if (damageable.Transform.TryGetComponent(out NavMeshAgent agent) && damageable.Transform.TryGetComponent<MonoBehaviour>(out MonoBehaviour behaviour))
            {
                behaviour.StartCoroutine(DoFreeze(agent));
            }
        }

        private IEnumerator DoFreeze(NavMeshAgent agent)
        {
            if (agent != null && agent.enabled && agent.isOnNavMesh)
            {
                agent.isStopped = true;
            }
            yield return new WaitForSeconds(SnareDuration);
            if (agent != null && agent.enabled && agent.isOnNavMesh)
            {
                agent.isStopped = false;
            }
        }
    }
}
