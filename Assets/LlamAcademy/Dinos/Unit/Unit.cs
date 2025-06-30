using LlamAcademy.Dinos.Enemy;
using LlamAcademy.Dinos.UI;
using LlamAcademy.Dinos.Utility;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

namespace LlamAcademy.Dinos.Unit
{
    public abstract class Unit : MonoBehaviour, IDamageable
    {
        [field: SerializeField] public int MaxHealth { get; set; }
        [field: SerializeField] public int Health { get; set; }

        public Transform Transform => transform;
        [field: SerializeField] public UnitSO UnitType { get; set; }
        [SerializeField] protected AttackRadius AttackRadius;
        [SerializeField] protected HealthBar HealthBar;
        public event IDamageable.TakeDamageEvent OnTakeDamage;
        public event IDamageable.DeathEvent OnDeath;

        public NavMeshAgent Agent { get; protected set; }
        protected Animator Animator;
        protected Rigidbody Rigidbody;
        protected BehaviorGraphAgent GraphAgent;
        private float LastSpeed;

        protected virtual void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();
            Animator = GetComponent<Animator>();
            GraphAgent = GetComponent<BehaviorGraphAgent>();
            Rigidbody = GetComponent<Rigidbody>();
        }

        protected virtual void Start()
        {
            if (GraphAgent != null)
            {
                GraphAgent.enabled = true;
            }

            if (Agent != null)
            {
                Agent.enabled = true;
            }

            if (AttackRadius != null)
            {
                AttackRadius.OnTargetEnter += OnTargetEnter;
                AttackRadius.OnTargetExit += OnTargetExit;
            }

            if (HealthBar == null)
            {
                HealthBar = GetComponentInChildren<HealthBar>();
            }
            if (HealthBar != null)
            {
                HealthBarCanvas.Instance.Register(HealthBar, this);
            }

            MaxHealth = UnitType.Health;
            Health = UnitType.Health;
        }

        protected abstract void OnTargetEnter(IDamageable target);
        protected abstract void OnTargetExit(IDamageable target);

        public virtual void TakeDamage(int damage)
        {
            if (damage >= Health)
            {
                Health = 0;
                if (HealthBar != null)
                {
                    HealthBar.SetProgress((float)Health / MaxHealth);
                }
                RaiseDamageEvent(Health);
                RaiseDeathEvent();
                Die();
            }
            else
            {
                Health -= damage;
                if (HealthBar != null)
                {
                    HealthBar.SetProgress((float)Health / MaxHealth);
                }
                RaiseDamageEvent(damage);
            }
        }

        protected virtual void Update()
        {
            if (Animator == null) return;

            Animator.SetFloat(AnimationConstants.SPEED_PARAMETER, Agent.enabled ? Agent.velocity.magnitude : 0);
        }

        public abstract void Die();

        protected void RaiseDamageEvent(int damage) => OnTakeDamage?.Invoke(this, damage);
        protected void RaiseDeathEvent() => OnDeath?.Invoke(this);

    }
}
