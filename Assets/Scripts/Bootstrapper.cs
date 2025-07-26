using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR;

public class Bootstrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void BeforeSceneLoad()
    {
#if UNITY_EDITOR
        Debug.Log("Bootstrapper: BeforeSceneLoad called.");
#endif

        RemoveOcclusionMask().Forget();
    }

    static async UniTask RemoveOcclusionMask()
    {
        XRDisplaySubsystem display = null;

        List<XRDisplaySubsystem> displaySubsystems = new List<XRDisplaySubsystem>();

        do
        {
            SubsystemManager.GetSubsystems(displaySubsystems);

            foreach (var d in displaySubsystems)
            {
                if (d.running)
                {
                    display = d;
                    break;
                }
            }
            await UniTask.Yield();
        } while (display == null);

        Debug.Log("RemoveOcclusionMask XRSettings.occlusionMaskScale = 0");
        XRSettings.occlusionMaskScale = 0;
        XRSettings.useOcclusionMesh = false;
    }
}
