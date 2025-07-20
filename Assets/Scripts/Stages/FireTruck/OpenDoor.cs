using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoroutineUtility;
using DG.Tweening;
using System.Threading.Tasks;
using Runtime.Common;

public class OpenDoor : Stage
{
    public GameObject door;
    public GameObject hintPoint;
    public GameObject informUi;

    [Header("門物件")]
    public Transform doorHandlerModel;   // 門把
    public Transform doorModel;  // 門

    Timer uiTimer;

    public override void OnBegin()
    {
        base.OnBegin();

        var trigger = door.AddComponent<TriggerEventHandler>();
        trigger.Register<NearCollisionFlag>(
            TriggerEventHandler.Timing.Stay,
            t =>
            {
                if (!t.ReadAsHandGripState())
                    return;

                DoorAnimation();
                trigger.enabled = false;
            });

        hintPoint.SetActive(true);

        uiTimer = new Timer(
            5f,
            () => informUi.SetActive(true),
            (f) => { },
            () => informUi.SetActive(false));

        GameHandler.Singleton.player.PathFinding(hintPoint.transform.position);
    }

    [ContextMenu("Test Door")]
    void DoorAnimation()
    {
        DOTween.Sequence()
            .Append(doorHandlerModel.DOLocalRotate(Vector3.back * 20, 1f, RotateMode.LocalAxisAdd))
            .Append(doorModel.DOLocalRotate(Vector3.up * 80, 1f, RotateMode.LocalAxisAdd))
            .OnComplete(() => isFinish = true);
    }

    public override void OnFinish()
    {
        base.OnFinish();

        hintPoint.SetActive(false);

        uiTimer.Stop(true);
        GameHandler.Singleton.player.line.gameObject.SetActive(false);
    }
}
