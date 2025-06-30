using System;
using LlamAcademy.Dinos.Unit;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace LlamAcademy.Dinos.Behavior
{
#if UNITY_EDITOR
    [CreateAssetMenu(menuName = "Behavior/Event Channels/Attack Event Channel")]
#endif
    [Serializable, GeneratePropertyBag]
    [EventChannelDescription(name: "Attack Event Channel", message: "[Self] performs an attack on [Target] .", category: "Events", id: "7696c7d3b286ca339bdda131f76b33a1")]
    public partial class AttackEventChannel : EventChannelBase
    {
        public delegate void AttackEventChannelEventHandler(GameObject Self, GameObject Target);
        public event AttackEventChannelEventHandler Event;

        public void SendEventMessage(GameObject Self, GameObject Target)
        {
            Event?.Invoke(Self, Target);
        }

        public override void SendEventMessage(BlackboardVariable[] messageData)
        {
            BlackboardVariable<GameObject> SelfBlackboardVariable = messageData[0] as BlackboardVariable<GameObject>;
            BlackboardVariable<GameObject> TargetBlackboardVariable = messageData[0] as BlackboardVariable<GameObject>;
            var Self = SelfBlackboardVariable != null ? SelfBlackboardVariable.Value : default(GameObject);
            var Target = TargetBlackboardVariable != null ? TargetBlackboardVariable.Value : default(GameObject);

            Event?.Invoke(Self, Target);
        }

        public override Delegate CreateEventHandler(BlackboardVariable[] vars, System.Action callback)
        {
            AttackEventChannelEventHandler del = (Self, Target) =>
            {
                if(vars[0] is BlackboardVariable<GameObject> var0)
                    var0.Value = Self;
                if (vars[1] is BlackboardVariable<GameObject> var1)
                    var1.Value = Target;

                callback();
            };
            return del;
        }

        public override void RegisterListener(Delegate del)
        {
            Event += del as AttackEventChannelEventHandler;
        }

        public override void UnregisterListener(Delegate del)
        {
            Event -= del as AttackEventChannelEventHandler;
        }
    }
}
