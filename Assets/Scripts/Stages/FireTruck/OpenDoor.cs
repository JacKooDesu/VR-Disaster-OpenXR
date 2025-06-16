using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoroutineUtility;
using DG.Tweening;
using System.Threading.Tasks;

public class OpenDoor : Stage
{
    public InteracableObject door;
    public GameObject hintPoint;
    public GameObject informUi;

    [Header("門物件")]
    public Transform doorHandlerModel;   // 門把
    public Transform doorModel;  // 門

    Timer uiTimer;

    public override void OnBegin()
    {
        base.OnBegin();

        door.Interactable = true;
        door.onHoverEvent.AddListener(() => DoorAnimation());

        hintPoint.SetActive(true);

        uiTimer = new Timer(
            5f,
            () => informUi.SetActive(true),
            (f) => { },
            () => informUi.SetActive(false));

        GameHandler.Singleton.player.PathFinding(hintPoint.transform.position);
    }

    async void DoorAnimation()
    {
        door.Interactable = false;

        doorHandlerModel.DOLocalRotate(Vector3.up * 20, .5f, RotateMode.LocalAxisAdd);
        await Task.Delay(500);

        doorModel.DOLocalRotate(Vector3.forward * 80, .5f, RotateMode.LocalAxisAdd);
        isFinish = true;
    }

    public override void OnFinish()
    {
        base.OnFinish();

        hintPoint.SetActive(false);

        uiTimer.Stop(true);
        GameHandler.Singleton.player.line.gameObject.SetActive(false);
    }
}
