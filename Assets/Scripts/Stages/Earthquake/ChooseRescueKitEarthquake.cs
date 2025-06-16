using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseRescueKitEarthquake : Stage
{
    public UIQuickSetting ui;
    public KitItem[] vipItem;   // 必須有的物件

    public MaterialChanger changer;

    public override void OnBegin()
    {
        base.OnBegin();
        FindObjectOfType<HintCanvas>().SetHintText("整理急難救助包！", true);
        JacDev.Audio.Earthquake audio = (JacDev.Audio.Earthquake)GameHandler.Singleton.audioHandler;
        audio.PlaySound(audio.selectWhistle);

        var player = GameHandler.Singleton.player;

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

        changer.ChangeColor();
    }

    // public void TakeWhistle()
    // {
    //     print("whistle");
    //     JacDev.Audio.Earthquake audio = (JacDev.Audio.Earthquake)GameHandler.Singleton.audioHandler;
    //     audio.PlaySound(audio.whistle);

    //     // StartCoroutine(
    //     //     GameHandler.Singleton.Counter(
    //     //         audio.whistle.length + 1,
    //     //         audio.whistle.length + 1,
    //     //         delegate
    //     //         {
    //     //             audio.PlaySound(audio.missionComplete);
    //     //         }
    //     //     )
    //     // );

    //     // StartCoroutine(
    //     //     GameHandler.Singleton.Counter(
    //     //         audio.whistle.length + 1 + audio.missionComplete.length + 1,
    //     //         audio.whistle.length + 1 + audio.missionComplete.length + 1,
    //     //         delegate
    //     //         {
    //     //             GameHandler.Singleton.StageFinish();
    //     //             ui.TurnOff();
    //     //         }
    //     //     )
    //     // );
    // }

    // public override void OnUpdate()
    // {
    //     base.OnUpdate();
    //     foreach (var item in requireItems)
    //     {
    //         if (!item.inPack)
    //             return;
    //     }

    //     isFinish = true;
    // }

    public override void OnFinish()
    {
        base.OnFinish();
        var player = GameHandler.Singleton.player;
        player.SetCanMove(true);
    }
}
