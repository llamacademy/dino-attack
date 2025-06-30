using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LlamAcademy.Dinos.Config;
using LlamAcademy.Dinos.Unit;
using LlamAcademy.Dinos.Utility;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Pool;
using Action = Unity.Behavior.Action;

namespace LlamAcademy.Dinos.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Attack Closest Object", story: "[Self] attacks [ClosestAttackable] until it dies.",
        category: "Action", id: "693534fc8c8f928bfb1b911de9ac3cf5")]
    public partial class AttackClosestObjectAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Self;
        [SerializeReference] public BlackboardVariable<GameObject> ClosestAttackable;
        [SerializeReference] public BlackboardVariable<AttackConfigSO> AttackConfig;
        [SerializeReference] public BlackboardVariable<float> RotationSpeed = new(5);
        [SerializeReference] public BlackboardVariable<float> LastAttackTime;
        [SerializeReference] public BlackboardVariable<AttackEventChannel> AttackEventChannel;
        [SerializeReference] public BlackboardVariable<DeathEventChannel> DeathEventChannel;

        private Transform Transform;
        private Vector3 TargetPosition;
        private IDamageable Damageable;
        private Collider DamageableCollider;
        private Animator Animator;
        private Dictionary<AttackTypeSO, float> AttackTimes = new();

        protected override Status OnStart()
        {
            if (AttackConfig.Value == null || Self.Value == null || ClosestAttackable.Value == null || !ClosestAttackable.Value.TryGetComponent(out Damageable)) return Status.Failure;

            Transform = Self.Value.transform;
            TargetPosition = GetTargetPosition();
            Animator = Self.Value.GetComponent<Animator>();

            foreach (AttackTypeSO attackType in AttackConfig.Value.AttackTypes)
            {
                AttackTimes.TryAdd(attackType, 0);
            }

            return Status.Running;
        }

        private Vector3 GetTargetPosition()
        {
            DamageableCollider = ClosestAttackable.Value.GetComponentInChildren<Collider>();

            if (DamageableCollider != null)
            {
                return DamageableCollider.ClosestPoint(Transform.position);
            }

            return ClosestAttackable.Value.transform.position;
        }

        protected override Status OnUpdate()
        {
            if (ClosestAttackable.Value == null) return Status.Failure;

            // TODO: Pick highest priority attack that is currently available.
            AttackTypeSO attack = AttackTimes
                .Where(attack => attack.Value + AttackConfig.Value.GetAttackDelay(attack.Key) < Time.time)
                .OrderBy(attack => attack.Key.Priority)
                .FirstOrDefault().Key;

            // keep rotating even if we're early aborting
            Quaternion lookRotation = Quaternion.LookRotation((TargetPosition - Transform.position).normalized);
            lookRotation = Quaternion.Euler(0, lookRotation.eulerAngles.y, 0); // force only y rotation
            Transform.rotation = Quaternion.Slerp(Transform.rotation, lookRotation, Time.deltaTime * RotationSpeed);

            if (attack == null) return Status.Running; // early abort if no attacks can be used

            TargetPosition = GetTargetPosition();
            if (XZDistanceIsGreaterThanMaxAttackDistance()) return Status.Failure;


            if (Animator != null)
            {
                Animator.SetBool(AnimationConstants.IS_ATTACKING_PARAMETER, true);
            }

            LastAttackTime.Value = Time.time;
            AttackTimes[attack] = Time.time;

            if (AttackEventChannel.Value != null)
            {
                AttackEventChannel.Value.SendEventMessage(Self.Value, Damageable.Transform.gameObject);
            }

            ObjectPool<ParticleSystem> particlePool = ParticleSystemHelper.Instance.GetParticleSystemPoolFor(attack);

            if (particlePool == null)
            {
                if (attack.DelayDamage)
                {
                    ParticleSystemHelper.Instance.StartCoroutine(DelayApplyEffect(attack, Damageable));
                }
                else
                {
                    ApplyDamage(attack, Damageable);
                }
            }

            if (particlePool != null)
            {
                if (attack.AttackParticleMoveConfig.MoveToTarget)
                {
                    ParticleSystem system = particlePool.Get();
                    system.transform.position = TargetPosition;
                    system.transform.LookAt(Transform.position + Vector3.up);
                    system.transform.position = Transform.position + attack.AttackParticleMoveConfig.LocalSpawnOffset;
                    // This will delay call attack.ApplyEffect
                    ParticleSystemHelper.Instance.MoveParticleSystemToTarget(Damageable.Transform, system, attack);
                }
                else if (attack.DelayDamage)
                {
                    ParticleSystemHelper.Instance.StartCoroutine(DelayApplyEffect(attack, Damageable, particlePool));
                }
                else
                {
                    ApplyDamage(attack, Damageable, particlePool);
                    // immediately apply any effects
                    attack.ApplyEffect(Damageable);
                }
            }

            if (Damageable.Health <= 0)
            {
                if (DeathEventChannel.Value != null)
                {
                    DeathEventChannel.Value.SendEventMessage(Self.Value, ClosestAttackable);
                }
                return Status.Success;
            }

            return Status.Running;
        }

        private void ApplyDamage(AttackTypeSO attack, IDamageable damageable, ObjectPool<ParticleSystem> particlePool = null)
        {
            if (damageable == null) return;

            if (particlePool != null)
            {
                ParticleSystem system = particlePool.Get();
                system.transform.position = TargetPosition;
                system.transform.LookAt(Transform.position + Vector3.up);
                ParticleSystemHelper.Instance.ReAddSystemToPoolOnDisable(particlePool, system);
            }

            attack.ApplyEffect(damageable);
        }

        private IEnumerator DelayApplyEffect(AttackTypeSO attack, IDamageable damageable, ObjectPool<ParticleSystem> particlePool = null)
        {
            yield return new WaitForSeconds(attack.DelayDamageTime);

            if (CurrentStatus == Status.Running && damageable != null)
            {
                ApplyDamage(attack, damageable, particlePool);
            }
        }

        private bool XZDistanceIsGreaterThanMaxAttackDistance()
        {
            Vector3 xzTargetPosition = new (TargetPosition.x, Transform.position.y, TargetPosition.z);
            // move to closest distance of any attack types, if they differ
            return Vector3.Distance(xzTargetPosition, Transform.position) >= AttackTimes.Min(kvp => kvp.Key.MaxAttackRange);
        }
    }
}
