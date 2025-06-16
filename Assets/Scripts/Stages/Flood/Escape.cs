using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Escape : Stage
{
    public GameObject water;
    public GameObject waterfall;
    public GameObject brokenCell;
    public UIQuickSetting hint;

    public override void OnBegin()
    {
        base.OnBegin();

        JacDev.Audio.Flood a = (JacDev.Audio.Flood)GameHandler.Singleton.audioHandler;
        a.PlaySound(a.waterIn);
        var player = GameHandler.Singleton.player;

        player.SetCanMove(false);


        GameHandler.Singleton.Counter(
            a.waterIn.length,
            delegate
            {
                a.PlaySound(a.broadcast2);
            }
        ).Forget();

        GameHandler.Singleton.Counter(
            a.waterIn.length + 5f,
                delegate
                {
                    a.GetSoundAudioSource(a.broadcast2).volume = .4f;
                    a.PlaySound(a.escape);

                    player.SetCanMove(true);
                }
            ).Forget();

        water.SetActive(true);
        iTween.MoveTo(water, Vector3.one * -1.46f, 12f);

        waterfall.SetActive(true);

        brokenCell.SetActive(false);
        onGetToTarget += () => isFinish = true;
    }

    public override void OnFinish()
    {
        JacDev.Audio.Flood a = (JacDev.Audio.Flood)GameHandler.Singleton.audioHandler;
        a.StopCurrent();
        a.PlaySound(a.stageClear);
        hint.TurnOn();
    }
}
