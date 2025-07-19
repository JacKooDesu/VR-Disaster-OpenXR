using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Common
{
    public class CollisionEventHandler : MonoBehaviour
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

        public static CollisionEventHandler Setup(GameObject target)
        {
            if (!target.GetComponent<Collider>())
            {
                Debug.Log($"[CollisionHandler] {target} not attach collider!");
                return null;
            }
            var result = target.AddComponent<CollisionEventHandler>();
            return result;
        }

        public CollisionEventHandler Register<T>(
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

        void Check(Timing timing, Collision other)
        {
            if (!hooks.TryGetValue(timing, out var dict))
                return;
            foreach (var (type, setting) in dict)
            {
                if (other.gameObject.TryGetComponent(type, out var component))
                    setting.Invoke(component);
            }
        }

        void OnCollisionEnter(Collision other) =>
            Check(Timing.Enter, other);

        void OnCollisionExit(Collision other) =>
            Check(Timing.Exit, other);

        void OnCollisionStay(Collision other) =>
            Check(Timing.Stay, other);
    }
}