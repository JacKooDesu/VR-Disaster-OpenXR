using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoroutineUtility
{
    public class Timer
    {
        static int globalId = 0;
        int id;
        float time;
        float current = 0;
        public string ID
        {
            get => $"timer-{id}";
        }
        bool hasRun = false;
        public bool HasRun
        {
            get => hasRun;
        }

        System.Action begin = () => { };
        System.Action<float> update = (f) => { };
        System.Action complete = () => { };

        public Timer(float time, System.Action complete, bool autoRun = true)
        {
            this.time = time;
            id = globalId;
            globalId++;

            this.complete = complete;

            if (autoRun)
                CoroutineManager.Singleton.Add(Run(time), $"timer-{id}");
        }

        public Timer(float time, System.Action begin, System.Action<float> update, System.Action complete, bool autoRun = true)
        {
            this.time = time;
            id = globalId;
            globalId++;

            this.begin = begin;
            this.update = update;
            this.complete = complete;

            if (autoRun)
                CoroutineManager.Singleton.Add(Run(time), $"timer-{id}");
        }

        IEnumerator Run(float time)
        {
            if (hasRun)
                yield break;

            hasRun = true;

            float t = 0;
            begin.Invoke();

            while (t < time)
            {
                if (update != null)
                    update.Invoke(t);
                t += Time.deltaTime;
                current = t;
                yield return null;
            }

            complete.Invoke();
        }

        public void Start()
        {
            CoroutineManager.Singleton.Add(Run(time), $"timer-{id}");
        }

        public void Stop(bool forceComplete = false)
        {
            if (!hasRun)
                return;

            if (forceComplete)
                complete.Invoke();
            CoroutineManager.Singleton.Stop($"timer-{id}");
            hasRun = false;
        }

        // WIP
        public void Pause()
        {

        }
    }
}
