using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class WalkInLibrary : Stage
{
    public float minTime;
    public float maxTime;

    public UIQuickSetting UI;

    public Animator elevator;

    public override void OnBegin()
    {
        // GameHandler.Singleton.player.SetCanMove(true);

        GameHandler.Singleton.MovePlayer(spawnpoint);

        UI.TurnOn();

        UI.WaitStatusChange(
            () =>
            {
                elevator.SetTrigger("Open");

                JacDev.Audio.Earthquake audio = (JacDev.Audio.Earthquake)GameHandler.Singleton.audioHandler;
                audio.PlayAudio(audio.libraryBgm, true);

                GameHandler.Singleton.Counter(minTime, maxTime, delegate { isFinish = true; }).Forget();
            }, false
        ).Forget();
    }

    public override void OnFinish()
    {
        elevator.SetTrigger("Close");
    }
}
