using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutLibrary : Stage
{
    public UIQuickSetting hint;

    public override void OnBegin()
    {
        base.OnBegin();
        JacDev.Audio.Earthquake audio = (JacDev.Audio.Earthquake)GameHandler.Singleton.audioHandler;
        audio.PlaySound(audio.missionComplete);
        hint.TurnOn();
        new CoroutineUtility.Timer(audio.missionComplete.length, () =>
        {
            isFinish = true;
        });

    }
}
