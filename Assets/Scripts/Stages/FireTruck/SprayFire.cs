using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprayFire : Stage
{
    public CustomControllerBehaviour controller;
    public ParticleSystem powder;
    [Header("UI設定")]
    public ObjectSwitcher uiSwitcher;
    public GameObject progressImage;
    CoroutineUtility.Timer uiTimer;


    public UIQuickSetting hint; // 超過時間
    // private variable
    JacDev.Audio.FireTruck audioHandler;
    public override void OnBegin()
    {
        base.OnBegin();

        audioHandler = (JacDev.Audio.FireTruck)GameHandler.Singleton.audioHandler;

        uiSwitcher.Switch(3);
        progressImage.SetActive(true);
        uiTimer = new CoroutineUtility.Timer(3f, () => uiSwitcher.HideAll());

        JacDev.Audio.FireTruck audio = (JacDev.Audio.FireTruck)GameHandler.Singleton.audioHandler;
        audio.StopCurrent();
        audio.PlaySound(audio.sprayTutorial);

        // 操作時間過長
        var playTimer = new CoroutineUtility.Timer(10f, ShowHint);
        onFinishEvent += () => playTimer.Stop();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        bool isPressing = XRInputManager.Instance.Button(controller.Device, null);
        if (isPressing)
        {
            if (!powder.isPlaying)
                powder.Play();

            audioHandler.PlayAudio(audioHandler.extinguisher, true, transform);
        }
        else
        {
            powder.Stop();
            if (GetComponentInChildren<AudioSource>())
                Destroy(GetComponentInChildren<AudioSource>().gameObject);
        }
    }

    async void ShowHint()
    {
        hint.TurnOn();
        SubScore(5);
        await System.Threading.Tasks.Task.Delay(2000);

        hint.TurnOff();
    }

    public override void OnFinish()
    {
        base.OnFinish();

        progressImage.SetActive(false);
        uiTimer.Stop(true);

        uiSwitcher.gameObject.SetActive(false);

        GameHandler.Singleton.player.SetCanMove(true);

        // 移除噴霧及音效
        powder.gameObject.SetActive(false);
        if (GetComponentInChildren<AudioSource>())
            Destroy(GetComponentInChildren<AudioSource>().gameObject);
    }
}
