using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressExtinguisher : Stage
{
    public CustomControllerBehaviour controller;

    [Header("UI設定")]
    public ObjectSwitcher uiSwitcher;
    public GameObject progressImage;
    CoroutineUtility.Timer uiTimer;


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
        bool isPressing = XRInputManager.Instance.Button(controller.Device, null);
        if (isPressing)
            isFinish = true;
    }

    public override void OnFinish()
    {
        base.OnFinish();

        progressImage.SetActive(false);
        uiTimer.Stop(true);
    }
}
