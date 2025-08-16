using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using Runtime.Common;

public class TurnOffGas : Stage
{
    public InteracableObject gasSwitch;
    public Transform switchModel;
    public GameObject fire;
    public UIQuickSetting hint;

    public override void OnBegin()
    {
        base.OnBegin();

        gasSwitch.Interactable = true;
        var trigger = gasSwitch.gameObject.AddComponent<TriggerEventHandler>();
        trigger.Register<NearCollisionFlag>(
            TriggerEventHandler.Timing.Stay,
            col =>
            {
                if (!gasSwitch.Interactable || !col.ReadAsHandGripState())
                    return;

                isFinish = true;
            });

        JacDev.Audio.Flood a = (JacDev.Audio.Flood)GameHandler.Singleton.audioHandler;
        AudioSource boil = a.PlayAudioLoop(a.boilWater, new()
        {
            ReturnTo = 3.85f,
            EndAt = a.boilWater.length
        }, fire.transform);
        boil.maxDistance = 10;
        boil.minDistance = 2;
        boil.transform.SetParent(default);

        GameHandler.Singleton.Counter(5, delegate
        {
            boil.volume = .4f;
            a.PlaySound(a.turnOffGas);
        }).Forget();

        onFinishEvent += () =>
        {
            Destroy(boil.gameObject);
            var gasOff = a.PlayAudio(a.gasOff, false, switchModel);
            var waterOff = a.PlayAudio(a.boilWaterOff, false, switchModel);

            gasOff.volume = .2f;
            gasOff.maxDistance = 1f;
            gasOff.minDistance = .2f;
            waterOff.volume = .2f;
            waterOff.maxDistance = 1f;
            waterOff.minDistance = .2f;
        };
        onGetToTarget += () =>
        {
            new CoroutineUtility.Timer(3f, hint.TurnOn, null, hint.TurnOff);
            GameHandler.Singleton.player.hintCanvas.ForceAlign();
        };

        switchModel.GetComponent<Outline>().enabled = true;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    public override void OnFinish()
    {
        base.OnFinish();
        gasSwitch.Interactable = false;
        fire.SetActive(false);
        switchModel.DORotate(Vector3.forward * 90, .5f, RotateMode.LocalAxisAdd);
        GameHandler.Singleton.player.line.gameObject.SetActive(false);

        switchModel.GetComponent<Outline>().enabled = false;
    }
}
