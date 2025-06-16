using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoroutineUtility;

public class FindFlashLight : Stage
{
    public GameObject pointLights;

    public GameObject flashlight;   // 手電筒
    public Light globalLight;   // 環境光源

    public override void OnBegin()
    {
        base.OnBegin();

        pointLights.SetActive(false);
        DG.Tweening.DOTween.To(
            () => globalLight.intensity, x => globalLight.intensity = x, .015f, .5f
        );

        JacDev.Audio.Flood a = (JacDev.Audio.Flood)GameHandler.Singleton.audioHandler;
        a.PlaySound(a.getRescueKit);

        var timer = new Timer(a.getRescueKit.length, () => FlashlightOn());
    }

    void FlashlightOn()
    {
        var light = flashlight.GetComponent<Light>();
        light.intensity = 0;
        flashlight.SetActive(true);
        DG.Tweening.DOTween.To(
           () => light.intensity, x => light.intensity = x, 1f, .5f
        );
        isFinish = true;
    }

    public override void OnFinish()
    {
        base.OnFinish();
        // JacDev.Audio.Flood a = (JacDev.Audio.Flood)GameHandler.Singleton.audioHandler;
        // a.StopAll();
    }
}
