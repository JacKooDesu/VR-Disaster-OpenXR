using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseRescueKitFlood : Stage
{
    public UIQuickSetting ui;
    public KitItem[] vipItem;   // 必須有的物件

    public override void OnBegin()
    {
        base.OnBegin();
        FindAnyObjectByType<HintCanvas>().SetHintText("整理急難救助包！", true);

        var player = GameHandler.Singleton.player;

        onGetToTarget += () =>
        {
            player.SetCanMove(false);
            ui.TurnOn();
            var btn = ui.GetComponentInChildren<Button>();
            btn.onClick.AddListener(() =>
            {
                player.kit.KitMissionSetup(
                            4,
                            i =>
                            {
                                SubScore((4 - i) * 5);
                                isFinish = true;
                            },
                            4, 2, vipItem);

                // btn.interactable = false;
                ui.TurnOff();
            }
            );
        };
    }

    public override void OnFinish()
    {
        base.OnFinish();
        var player = GameHandler.Singleton.player;
        player.SetCanMove(true);
        // player.kit.ForceStopMission();
    }
}
