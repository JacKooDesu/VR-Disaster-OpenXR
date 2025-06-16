using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PullTorus : Stage
{
    public GameObject torus;
    public Transform handPosition;
    public GameObject extinguisher;

    [Header("UI設定")]
    public ObjectSwitcher uiSwitcher;
    public GameObject progressImage;
    CoroutineUtility.Timer uiTimer;
    public MaterialChanger changer;

    public override async void OnBegin()
    {
        base.OnBegin();
        uiSwitcher.gameObject.SetActive(true);
        await System.Threading.Tasks.Task.Yield();
        changer.ChangeColor();
        uiSwitcher.Switch(0);
        progressImage.SetActive(true);
        uiTimer = new CoroutineUtility.Timer(3f, () => uiSwitcher.HideAll());

        torus.GetComponent<Outline>().enabled = true;
        var torusInteract = torus.GetComponent<InteracableObject>();
        torusInteract.enabled = true;
        torusInteract.onHoverEvent.AddListener(() => isFinish = true);

        JacDev.Audio.FireTruck audio = (JacDev.Audio.FireTruck)GameHandler.Singleton.audioHandler;
        audio.StopCurrent();
        audio.PlaySound(audio.pullTutorial);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    public override void OnFinish()
    {
        base.OnFinish();
        progressImage.SetActive(false);
        uiTimer.Stop(true);
        changer.BackOriginColor();

        float torusZ = torus.transform.localPosition.z;
        torus.transform.DOLocalMoveZ(torusZ + .2f, 1f).OnComplete(() =>
        {
            torus.SetActive(false);
            extinguisher.transform.DOScale(Vector3.zero, .5f).OnComplete(
                () => extinguisher.SetActive(false)
            ).SetEase(Ease.InBack);
        });
        // torus.GetComponent<Outline>().enabled = false;
        // var torusInteract = torus.GetComponent<InteracableObject>();
        // torusInteract.onHoverEvent.RemoveAllListeners();
        // torusInteract.enabled = false;
    }
}
