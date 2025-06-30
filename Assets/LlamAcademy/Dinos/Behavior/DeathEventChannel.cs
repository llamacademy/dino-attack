using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace LlamAcademy.Dinos.Behavior
{
    [CreateAssetMenu(menuName = "Behavior/Event Channels/Death Event Channel")]
    [Serializable, GeneratePropertyBag]
    [EventChannelDescription(name: "Death Event Channel", message: "[Self] kills [Attackable] .", category: "Events", id: "8fce080c8a947ee6ead6b52895a84a78")]
    public partial class DeathEventChannel : EventChannelBase
    {
        public delegate void DeathEventChannelEventHandler(GameObject Self, GameObject Attackable);
        public event DeathEventChannelEventHandler Event;

        public void SendEventMessage(GameObject Self, GameObject Attackable)
        {
            Event?.Invoke(Self, Attackable);
        }

        public override void SendEventMessage(BlackboardVariable[] messageData)
        {
            BlackboardVariable<GameObject> SelfBlackboardVariable = messageData[0] as BlackboardVariable<GameObject>;
            var Self = SelfBlackboardVariable != null ? SelfBlackboardVariable.Value : default(GameObject);

            BlackboardVariable<GameObject> AttackableBlackboardVariable = messageData[1] as BlackboardVariable<GameObject>;
            var Attackable = AttackableBlackboardVariable != null ? AttackableBlackboardVariable.Value : default(GameObject);

            Event?.Invoke(Self, Attackable);
        }

        public override Delegate CreateEventHandler(BlackboardVariable[] vars, System.Action callback)
        {
            DeathEventChannelEventHandler del = (Self, Attackable) =>
            {
                BlackboardVariable<GameObject> var0 = vars[0] as BlackboardVariable<GameObject>;
                if(var0 != null)
                    var0.Value = Self;

                BlackboardVariable<GameObject> var1 = vars[1] as BlackboardVariable<GameObject>;
                if(var1 != null)
                    var1.Value = Attackable;

                callback();
            };
            return del;
        }

        public override void RegisterListener(Delegate del)
        {
            Event += del as DeathEventChannelEventHandler;
        }

        public override void UnregisterListener(Delegate del)
        {
            Event -= del as DeathEventChannelEventHandler;
        }
    }
}
