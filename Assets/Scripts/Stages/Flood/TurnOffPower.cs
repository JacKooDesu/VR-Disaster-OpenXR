using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TurnOffPower : Stage
{
    public Transform electronicBoxDoor;
    public Transform switchModel;
    public InteracableObject doorInteract;
    public InteracableObject switchInteract;
    public UIQuickSetting hint;

    public override void OnBegin()
    {
        base.OnBegin();
        JacDev.Audio.Flood a = (JacDev.Audio.Flood)GameHandler.Singleton.audioHandler;
        a.PlaySound(a.turnOffSwitch);

        doorInteract.Interactable = true;
        doorInteract.onHoverEvent.AddListener(
            () =>
            {
                switchInteract.Interactable = true;
                doorInteract.Interactable = false;
                electronicBoxDoor.DORotate(Vector3.down * 180, 1f, RotateMode.WorldAxisAdd);
                electronicBoxDoor.GetComponent<Outline>().enabled = false;
                switchModel.GetComponent<Outline>().enabled = true;
            }
        );

        switchInteract.Interactable = false;
        switchInteract.onHoverEvent.AddListener(
            () =>
            {
                a.PlayAudio(a.switchSound, false, switchInteract.transform);
                switchInteract.Interactable = false;
                switchModel.DORotate(Vector3.back * 60, .2f, RotateMode.LocalAxisAdd);
                isFinish = true;
                switchModel.GetComponent<Outline>().enabled = false;
            }
        );


        electronicBoxDoor.GetComponent<Outline>().enabled = true;

        onGetToTarget += () =>
        {
            new CoroutineUtility.Timer(3f, hint.TurnOn, null, hint.TurnOff);
            GameHandler.Singleton.player.hintCanvas.ForceAlign();
        };
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
