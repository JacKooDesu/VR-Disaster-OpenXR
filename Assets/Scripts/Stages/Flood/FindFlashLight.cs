using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoroutineUtility;
using System.Linq;

public class FindFlashLight : Stage
{
    public GameObject flashlight;   // 手電筒
    public Light globalLight;   // 環境光源
    public Light[] _roomLights;

    [SerializeField] Texture2D[] _lowLightTextures;
    [SerializeField] Texture2D[] _lowDirTextures;

    public override void OnBegin()
    {
        base.OnBegin();

        DG.Tweening.DOTween.To(
            () => globalLight.intensity, x => globalLight.intensity = x, .015f, .5f
        );

        foreach (var light in _roomLights)
            light.enabled = false;

        var lightmaps = LightmapSettings.lightmaps.ToArray();
        for (int i = 0; i < lightmaps.Length; ++i)
        {
            lightmaps[i].lightmapColor = _lowLightTextures[0];
            lightmaps[i].lightmapDir = _lowDirTextures[0];
        }
        LightmapSettings.lightmaps = lightmaps;

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
