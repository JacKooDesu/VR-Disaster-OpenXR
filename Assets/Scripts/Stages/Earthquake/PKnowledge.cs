using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PKnowledge : Stage
{
    public UIQuickSetting UI;
    public UIQuickSetting waringHUD;

    public Transform tableTop;

    public override void OnBegin()
    {
        if (!UI.gameObject.activeInHierarchy)
            UI.gameObject.SetActive(true);

        UI.TurnOn();

        UI.WaitStatusChange(delegate
        {
            isFinish = true;
        }, false).Forget();
    }

    public override void OnUpdate()
    {
        if (GameHandler.Singleton.cam.transform.position.y > tableTop.position.y)
        {
            if (!waringHUD.gameObject.activeInHierarchy)
            {
                waringHUD.gameObject.SetActive(true);
            }
            waringHUD.TurnOn();
        }
        else
        {
            waringHUD.TurnOff();
        }
    }

    public override void OnFinish()
    {
        base.OnFinish();
    }
}
