using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Common
{
    public class TriggerEventHandler : MonoBehaviour
    {
        public abstract class Setting
        {
            public abstract void Invoke<T>(T component) where T : Component;
        }
        public class Setting<T> : Setting
        where T : Component
        {
            internal UnityEvent<T> Event { get; } = new();
            public override void Invoke<_>(_ t) => Event.Invoke(t as T);
        }

        public enum Timing
        {
            Enter,
            Stay,
            Exit
        }
        Dictionary<Timing, Dictionary<Type, Setting>> hooks = new();

        public static TriggerEventHandler Setup(GameObject target)
        {
            if (!target.GetComponent<Collider>())
            {
                Debug.Log($"[CollisionHandler] {target} not attach collider!");
                return null;
            }
            var result = target.AddComponent<TriggerEventHandler>();
            return result;
        }

        public TriggerEventHandler Register<T>(
            Timing timing,
            UnityAction<T> action
        )
        where T : Component
        {
            Dictionary<Type, Setting> dict;
            if (!hooks.TryGetValue(timing, out dict))
                hooks.Add(timing, dict = new());

            var component = typeof(T);
            Setting<T> target;
            if (dict.ContainsKey(component))
                target = dict[component] as Setting<T>;
            else
            {
                target = new();
                dict.Add(component, target as Setting);
            }

            target.Event.AddListener(action);
            return this;
        }

        public TriggerEventHandler Register<TSelf, T>(
            Timing timing,
            UnityAction<TSelf, T> action
        )
        where T : Component
        {
            Dictionary<Type, Setting> dict;
            if (!hooks.TryGetValue(timing, out dict))
                hooks.Add(timing, dict = new());

            var component = typeof(T);
            Setting<T> target;
            if (dict.ContainsKey(component))
                target = dict[component] as Setting<T>;
            else
            {
                target = new();
                dict.Add(component, target as Setting);
            }

            target.Event.AddListener(t => action(GetComponent<TSelf>(), t));
            return this;
        }

        void Check(Timing timing, Collider other)
        {
            if (!hooks.TryGetValue(timing, out var dict))
                return;
            foreach (var (type, setting) in dict)
            {
                if (other.TryGetComponent(type, out var component))
                    setting.Invoke(component);
            }
        }

        void OnTriggerEnter(Collider other) =>
            Check(Timing.Enter, other);

        void OnTriggerExit(Collider other) =>
            Check(Timing.Exit, other);

        void OnTriggerStay(Collider other) =>
            Check(Timing.Stay, other);
    }
}