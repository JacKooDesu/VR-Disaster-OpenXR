using System.Collections;
using System.Collections.Generic;
using Runtime.Common;
using UnityEngine;
using UnityEngine.UI;

public class HoldOn : Stage
{
    public ObjectSwitcher uiSwitcher;
    public Image progressImage;
    public Transform tableLower;
    [SerializeField]
    GameObject _holdArea;

    public MaterialChanger changer;
    CoroutineUtility.Timer uiTimer;

    public GameObject dchUI;

    TriggerEventHandler _tableTrigger;
    bool _l, _r;

    public override void OnBegin()
    {
        base.OnBegin();
        GameHandler.Singleton.player.SetCanMove(false);
        // XRActionGestureManager.ActionDetectedEvent += CheckHandHoldingEvent;
        changer.ChangeColor();

        uiSwitcher.Switch(2);
        progressImage.color = Color.white;

        uiTimer = new CoroutineUtility.Timer(3f, () => uiSwitcher.HideAll());

        _tableTrigger = _holdArea.AddComponent<TriggerEventHandler>()
            .Register<NearCollisionFlag>(TriggerEventHandler.Timing.Enter, Enter)
            .Register<NearCollisionFlag>(TriggerEventHandler.Timing.Exit, Exit)
            ;
    }

    void Enter(NearCollisionFlag col)
    {
        var p = GameHandler.Singleton.player;
        _l = col.transform.parent == p.leftHandler ? true : _l;
        _r = col.transform.parent == p.rightHandler ? true : _r;
    }

    void Exit(NearCollisionFlag col)
    {
        var p = GameHandler.Singleton.player;
        _l = col.transform.parent == p.leftHandler ? false : _l;
        _r = col.transform.parent == p.rightHandler ? false : _r;
    }

    public override void OnUpdate()
    {
        var p = GameHandler.Singleton.player;
        if (_l && _r &&
            p.State_LGrip &&
            p.State_RGrip)
            isFinish = true;
        // if (GameHandler.Singleton.cam.transform.position.y > tableTop.position.y)
        // {
        //     if (!waringHUD.gameObject.activeInHierarchy)
        //     {
        //         waringHUD.gameObject.SetActive(true);
        //     }
        //     waringHUD.TurnOn();
        //     GameHandler.Singleton.BlurCamera(true);
        // }
        // else
        // {

        //     waringHUD.TurnOff();
        //     GameHandler.Singleton.BlurCamera(false);
        // }
    }

    public override void OnFinish()
    {
        base.OnFinish();
        _tableTrigger.enabled = false;
        // XRActionGestureManager.ActionDetectedEvent -= CheckHandHoldingEvent;

        // UI.TurnOff();

        // 2021.03.11 
        // UI.transform.parent.gameObject.SetActive(false);
        // GameHandler.Singleton.cam.GetComponent<UnityStandardAssets.ImageEffects.Grayscale>().enabled = false;

        // tweener.MoveNextPoint();

        changer.BackOriginColor();
        progressImage.color = Color.gray;
        uiTimer.Stop(true);

        dchUI.SetActive(false);
    }
}
