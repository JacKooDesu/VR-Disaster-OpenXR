using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoroutineUtility;

public class OutBus : Stage
{
    public GameObject finishHint;
    Timer uiTimer;
    public override void OnBegin()
    {
        base.OnBegin();
        JacDev.Audio.FireTruck audio = (JacDev.Audio.FireTruck)GameHandler.Singleton.audioHandler;
        audio.PlaySound(audio.finish);

        uiTimer = new Timer(
            audio.finish.length,
            () => finishHint.SetActive(true),
            (f) => { },
            () => isFinish = true);
    }

    public override void OnFinish()
    {
        base.OnFinish();
        finishHint.SetActive(false);
    }
}
