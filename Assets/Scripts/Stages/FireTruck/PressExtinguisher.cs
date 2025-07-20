using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressExtinguisher : Stage
{
    const float THRESHOLD = FireTruck_Constants.EXTINGUISHER_TRIGGER_THRESHOLD;
    public CustomControllerBehaviour controller;

    [Header("UI設定")]
    public ObjectSwitcher uiSwitcher;
    public GameObject progressImage;
    CoroutineUtility.Timer uiTimer;

    [SerializeField]
    Transform _fire;

    public override void OnBegin()
    {
        base.OnBegin();

        uiSwitcher.Switch(2);
        progressImage.SetActive(true);
        uiTimer = new CoroutineUtility.Timer(3f, () => uiSwitcher.HideAll());

        JacDev.Audio.FireTruck audio = (JacDev.Audio.FireTruck)GameHandler.Singleton.audioHandler;
        audio.StopCurrent();
        audio.PlaySound(audio.pressTutorial);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        var lPress = GameHandler.Singleton.player.State_LTrigger > 0;
        var rPress = GameHandler.Singleton.player.State_RTrigger > 0;

        var lHand = GameHandler.Singleton.player.leftHandler;
        var rHand = GameHandler.Singleton.player.rightHandler;

        var lCast = Physics.Raycast(new Ray(lHand.position, lHand.forward), out var lHit, 10f);
        var rCast = Physics.Raycast(new Ray(rHand.position, rHand.forward), out var rHit, 10f);

        if (lCast && lHit.transform == _fire && rPress && !lPress)
            isFinish = true;

        if (rCast && rHit.transform == _fire && lPress && !rPress)
            isFinish = true;
    }

    public override void OnFinish()
    {
        base.OnFinish();

        progressImage.SetActive(false);
        uiTimer.Stop(true);
    }
}
