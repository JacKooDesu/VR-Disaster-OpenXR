using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Runtime.Common;

public class TurnOffPower : Stage
{
    public Transform electronicBoxDoor;
    public Transform switchModel;
    public InteracableObject doorInteract;
    public InteracableObject switchInteract;
    public UIQuickSetting hint;
    JacDev.Audio.Flood a;

    public override void OnBegin()
    {
        base.OnBegin();
        a = (JacDev.Audio.Flood)GameHandler.Singleton.audioHandler;
        a.PlaySound(a.turnOffSwitch);

        doorInteract.Interactable = true;
        switchInteract.Interactable = false;

        BindDoor();
        BindSwitch();

        electronicBoxDoor.GetComponent<Outline>().enabled = true;

        onGetToTarget += () =>
        {
            new CoroutineUtility.Timer(3f, hint.TurnOn, null, hint.TurnOff);
            GameHandler.Singleton.player.hintCanvas.ForceAlign();
        };
    }

    void BindDoor()
    {
        var trigger = doorInteract.gameObject.AddComponent<TriggerEventHandler>();
        trigger.Register<NearCollisionFlag>(
            TriggerEventHandler.Timing.Stay,
            col =>
            {
                if (!doorInteract.Interactable || !col.ReadAsHandGripState())
                    return;

                doorInteract.gameObject.SetActive(false);
                doorInteract.Interactable = false;
                electronicBoxDoor.GetComponent<Outline>().enabled = false;
                switchModel.GetComponent<Outline>().enabled = true;
                electronicBoxDoor.DORotate(Vector3.down * 180, 1f, RotateMode.WorldAxisAdd)
                    .OnComplete(() => switchInteract.Interactable = true);
            }
        );
    }

    void BindSwitch()
    {

        var trigger = switchInteract.gameObject.AddComponent<TriggerEventHandler>();
        trigger.Register<NearCollisionFlag>(
            TriggerEventHandler.Timing.Stay,
            col =>
            {
                if (!switchInteract.Interactable || !col.ReadAsHandGripState())
                    return;

                a.PlayAudio(a.switchSound, false, switchInteract.transform);
                switchInteract.gameObject.SetActive(false);
                switchInteract.Interactable = false;
                switchModel.DORotate(Vector3.back * 60, .2f, RotateMode.LocalAxisAdd);
                isFinish = true;
                switchModel.GetComponent<Outline>().enabled = false;
            });
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    public override void OnFinish()
    {
        base.OnFinish();
    }
}
