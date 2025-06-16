using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloodInform : Stage
{
    public UIQuickSetting ui;
    public override void OnBegin()
    {
        base.OnBegin();
        ui.TurnOn();
        GameHandler.Singleton.player.SetCanMove(false);
    }

    public override void OnFinish()
    {
        base.OnFinish();

        GameHandler.Singleton.player.SetCanMove(true);
    }
}
