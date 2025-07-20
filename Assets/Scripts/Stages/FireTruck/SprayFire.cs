using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SprayFire : Stage
{
    const float THRESHOLD = FireTruck_Constants.EXTINGUISHER_TRIGGER_THRESHOLD;
    public CustomControllerBehaviour controller;
    public ParticleSystem powder;
    [Header("UI設定")]
    public ObjectSwitcher uiSwitcher;
    public GameObject progressImage;
    CoroutineUtility.Timer uiTimer;


    public UIQuickSetting hint; // 超過時間
    // private variable
    JacDev.Audio.FireTruck audioHandler;

#if UNITY_EDITOR
    [SerializeField]
    bool _debugMode = false;
    [SerializeField, Range(0f, 1f)]
    float _debugL, _debugR;
#endif
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
        var lValue = GameHandler.Singleton.player.State_LTrigger;
        var rValue = GameHandler.Singleton.player.State_RTrigger;
#if UNITY_EDITOR
        if (_debugMode)
        {
            lValue = _debugL;
            rValue = _debugR;
        }
#endif
        var finalValue = 0f;

        if (lValue > THRESHOLD && rValue > THRESHOLD)
            finalValue = 0;
        else if (lValue > THRESHOLD)
        {
            finalValue = lValue;
            BindPowderToTransform(GameHandler.Singleton.player.rightHandler);
        }
        else if (rValue > THRESHOLD)
        {
            finalValue = rValue;
            BindPowderToTransform(GameHandler.Singleton.player.leftHandler);
        }

        if (finalValue > 0)
        {
            var e = powder.emission;
            e.rateOverTime = math.lerp(0, 30, finalValue);
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

    void BindPowderToTransform(Transform t)
    {
        powder.transform.SetParent(t);
        powder.transform.localPosition = Vector3.zero;
        powder.transform.localRotation = Quaternion.identity;
    }
}
