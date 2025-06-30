using System;
using System.Collections;
using System.Collections.Generic;
using LlamAcademy.Dinos.Behavior;
using LlamAcademy.Dinos.RoundManagement;
using LlamAcademy.Dinos.Utility;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

namespace LlamAcademy.Dinos.Unit
{
    [RequireComponent(typeof(NavMeshAgent), typeof(Animator), typeof(BehaviorGraphAgent))]
    [RequireComponent(typeof(Rigidbody))]
    public class Dino : Unit
    {
        [SerializeField] private Material DefaultMaterial;
        [SerializeField] private ChainIKConstraint ChainIKConstraint;
        [SerializeField] private IKConstraintData IKConstraint;
        private IDamageable Target;
        private Vector3 HitTarget;

        private Coroutine IKCoroutine;

        protected override void Start()
        {
            base.Start();
            // don't allow the agent to do stuff until we click go!
            GraphAgent.enabled = false;
            RoundManager.Instance.OnGameStateChange += InstanceOnOnGameStateChange;

            SetDestination(transform.position);
            GraphAgent.SetVariableValue(DinoGraphConstants.POST_ATTACK_COOLDOWN, UnitType.AttackConfig.PostAttackCooldown);
            if (ChainIKConstraint != null && GraphAgent.GetVariable(DinoGraphConstants.ATTACK_EVENT_CHANNEL, out BlackboardVariable<AttackEventChannel> attackEventChannelVariable))
            {
                // In Behavior 1.0.10 simply doing attackEventChannelVariable.Value.Event += HandleAttack; became inaccessible.
                // This is a workaround to that. It is planned to be re-added.
                Action<GameObject, GameObject> target = HandleAttack;
                attackEventChannelVariable.Value.RegisterListener(Delegate.CreateDelegate(typeof(AttackEventChannel.AttackEventChannelEventHandler), target.Target, target.Method));
            }

            RestoreDefaultMaterial();
        }

        private void HandleAttack(GameObject self, GameObject target)
        {
            if (ChainIKConstraint != null)
            {
                Vector3 targetPosition = target.transform.position + target.transform.forward * 0.5f;
                ChainIKConstraint.data.target.position = targetPosition + Vector3.up * 1.5f;;
                if (IKCoroutine != null)
                {
                    StopCoroutine(IKCoroutine);
                }
                IKCoroutine = StartCoroutine(LerpChainIKWeight());
            }
        }

        private IEnumerator LerpChainIKWeight()
        {
            float time = Time.deltaTime;
            while (time < 1)
            {
                time += Time.deltaTime / IKConstraint.LerpTime;
                ChainIKConstraint.weight += IKConstraint.MaxWeight * Time.deltaTime;
                yield return null;
            }

            ChainIKConstraint.weight = IKConstraint.MaxWeight;

            yield return new WaitForSeconds(IKConstraint.FullWeightDuration);
            time = 0;
            while (time < 1)
            {
                time -= Time.deltaTime / IKConstraint.LerpTime;
                ChainIKConstraint.weight -= IKConstraint.MaxWeight * Time.deltaTime;
                yield return null;
            }

            ChainIKConstraint.weight = 0;
        }

        private void InstanceOnOnGameStateChange(GameState oldstate, GameState newstate)
        {
            if (newstate == GameState.Running)
            {
                GraphAgent.enabled = true;
            }
        }

        protected override void OnTargetExit(IDamageable target)
        {
            if (!GraphAgent.GetVariable(DinoGraphConstants.NEARBY_ATTACKABLES, out BlackboardVariable<List<GameObject>> nearbyAttackables)) return;

            nearbyAttackables.Value.Remove(target.Transform.gameObject);
            nearbyAttackables.Value.RemoveAll((item) => item == null);
            GraphAgent.SetVariableValue(DinoGraphConstants.NEARBY_ATTACKABLES, nearbyAttackables.Value);
        }

        protected override void OnTargetEnter(IDamageable target)
        {
            if (!GraphAgent.GetVariable(DinoGraphConstants.NEARBY_ATTACKABLES, out BlackboardVariable<List<GameObject>> nearbyAttackables)) return;

            nearbyAttackables.Value.Add(target.Transform.gameObject);
            GraphAgent.SetVariableValue(DinoGraphConstants.NEARBY_ATTACKABLES, nearbyAttackables.Value);
        }

        private void OnEnable()
        {
            Agent.Warp(transform.position);
            Agent.enabled = true;
        }

        private void OnDisable()
        {
            Agent.enabled = false;
            RoundManager.Instance.OnGameStateChange -= InstanceOnOnGameStateChange;
        }

        public override void Die()
        {
            if (Animator != null)
            {
                Animator.SetTrigger(AnimationConstants.DIE_PARAMETER);
            }

            if (Agent != null)
            {
                Agent.enabled = false;
            }

            if (GraphAgent != null)
            {
                GraphAgent.enabled = false;
            }

            Invoke(nameof(DestroyGO), 2.5f);
        }

        private void DestroyGO()
        {
            Destroy(gameObject);
        }

        public void SetDestination(Vector3 target)
        {
            GraphAgent.SetVariableValue(DinoGraphConstants.EGG_LOCATION, target);
        }

        public void DealDamage()
        {
            // play some particles or something
        }

        private void RestoreDefaultMaterial()
        {
            GetComponentInChildren<Renderer>().material = DefaultMaterial;
        }

        [Serializable]
        private struct IKConstraintData
        {
            [field: SerializeField] public float LerpTime { get; private set; }
            [field: SerializeField] public float MaxWeight { get; private set; }
            [field: SerializeField] public float FullWeightDuration { get; private set; }
        }
    }
}
