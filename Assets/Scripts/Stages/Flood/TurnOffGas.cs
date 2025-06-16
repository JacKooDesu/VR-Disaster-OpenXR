using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Cysharp.Threading.Tasks;

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
        gasSwitch.onHoverEvent.AddListener(() => isFinish = true);

        JacDev.Audio.Flood a = (JacDev.Audio.Flood)GameHandler.Singleton.audioHandler;
        AudioSource boil = a.PlaySound(a.boilWater);
        GameHandler.Singleton.Counter(5, delegate
       {
           boil.volume = .4f;
           a.PlaySound(a.turnOffGas);
       }).Forget();

        onFinishEvent += () => boil.Stop();
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
