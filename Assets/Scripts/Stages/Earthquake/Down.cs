using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.ImageEffects;
using UnityEngine.UI;

public class Down : Stage
{
    public ObjectSwitcher uiSwitcher;
    [SerializeField]
    float height = 1.3f;
    public Image progressImage;

    public MaterialChanger changer;

    CoroutineUtility.Timer uiTimer;

    public override void OnBegin()
    {
        base.OnBegin();
        GameHandler.Singleton.player.hintCanvas.SetHintText("請趴低找掩護", true, true);
        changer.ChangeColor();

        uiSwitcher.Switch(0);
        uiTimer = new CoroutineUtility.Timer(3f, () => uiSwitcher.HideAll());

        progressImage.color = Color.white;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (GameHandler.Singleton.player.head.position.y < height)
        {
            isFinish = true;
        }
    }

    public override void OnFinish()
    {
        base.OnFinish();
        // UI.TurnOff();
        // tweener.MoveNextPoint();
        changer.BackOriginColor();
        uiTimer.Stop(true);

        progressImage.color = Color.gray;
    }
}
