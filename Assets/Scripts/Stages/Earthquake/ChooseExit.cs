using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ChooseExit : Stage
{
    public UIQuickSetting failedUI;

    Vector3 originPlayerPosition;

    public Animator[] exits;

    public MaterialChanger changer;

    public override void OnBegin()
    {
        base.OnBegin();
        originPlayerPosition = GameHandler.Singleton.player.transform.position;

        GameHandler.Singleton.player.SetCanMove(true);

        JacDev.Audio.Earthquake audio = (JacDev.Audio.Earthquake)GameHandler.Singleton.audioHandler;
        audio.PlaySound(audio.protectHead);

        FindAnyObjectByType<HintCanvas>().SetHintText("注意掉落物，小心頭部安全！", true);

        foreach (TeleportPoint tp in FindStageObjects<TeleportPoint>("逃生門"))
        {
            tp.onTeleportAction.AddListener(() =>
            {
                GameHandler.Singleton.StageFinish();
            });
        }

        foreach (TeleportPoint tp in FindStageObjects<TeleportPoint>("電梯"))
        {
            tp.onTeleportAction.AddListener(() =>
            {
                SubScore(10);
                GameHandler.Singleton.player.Teleport(originPlayerPosition);
            });
        }

        changer.ChangeColor();

    }

    public override void OnUpdate()
    {
    }

    public override void OnFinish()
    {
        base.OnFinish();
        foreach (Animator a in exits)
        {
            a.enabled = true;
        }

        changer.BackOriginColor();
    }
}
