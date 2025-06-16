using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimFire : Stage
{
    public Transform firespot;
    public CustomControllerBehaviour controller;
    [Header("UI設定")]
    public ObjectSwitcher uiSwitcher;
    public GameObject progressImage;
    CoroutineUtility.Timer uiTimer;
    public MaterialChanger changer;

    bool isNearFire = false;

    public override void OnBegin()
    {
        base.OnBegin();

        // GameHandler.Singleton.player.SetCanMove(false);

        uiSwitcher.Switch(1);
        progressImage.SetActive(true);
        uiTimer = new CoroutineUtility.Timer(3f, () => uiSwitcher.HideAll());

        changer.ChangeColor();

        // FindObjectOfType<HintCanvas>().SetHintText("靠近一點火源", true);

        JacDev.Audio.FireTruck audio = (JacDev.Audio.FireTruck)GameHandler.Singleton.audioHandler;
        audio.StopCurrent();
        audio.PlaySound(audio.aimTutorial);

        onGetToTarget += () =>
        {
            isNearFire = true;
            GameHandler.Singleton.player.SetCanMove(false);
            FindObjectOfType<HintCanvas>().SetHintText("瞄準火源", true);
        };
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (!isNearFire)
            return;

        Transform origin = controller.transform;

        RaycastHit hit;
        if (origin != null)
        {
            Ray ray = new Ray(origin.position, origin.forward);
            if (Physics.Raycast(ray, out hit, 10f))
            {
                // print(hit.transform.name);
                if (hit.transform == firespot)
                {
                    isFinish = true;
                }
            }
        }
    }

    public override void OnFinish()
    {
        base.OnFinish();

        progressImage.SetActive(false);
        uiTimer.Stop(true);

        changer.BackOriginColor();
    }
}
