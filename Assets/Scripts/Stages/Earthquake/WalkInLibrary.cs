using System.Collections;
using System.Collections.Generic;
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

        StartCoroutine(UI.WaitStatusChange(
            () =>
            {
                elevator.SetTrigger("Open");

                JacDev.Audio.Earthquake audio = (JacDev.Audio.Earthquake)GameHandler.Singleton.audioHandler;
                audio.PlayAudio(audio.libraryBgm, true);

                StartCoroutine(GameHandler.Singleton.Counter(minTime, maxTime, delegate { isFinish = true; }));
            }, false
        ));
    }

    public override void OnFinish()
    {
        elevator.SetTrigger("Close");
    }
}
