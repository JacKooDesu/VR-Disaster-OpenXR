using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cover : Stage
{
    public ObjectSwitcher uiSwitcher;
    public Image progressImage;
    [SerializeField] Transform table;
    [SerializeField] float radius;
    public MaterialChanger changer;
    CoroutineUtility.Timer uiTimer;

    public override void OnBegin()
    {
        base.OnBegin();
        changer.ChangeColor();
        uiSwitcher.Switch(1);
        progressImage.color = Color.white;
        uiTimer = new CoroutineUtility.Timer(3f, () => uiSwitcher.HideAll());
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        var headPos = GameHandler.Singleton.player.head.position;
        var targetPos = table.position;
        if ((headPos - targetPos).magnitude < radius)
            isFinish = true;

    }

    public override void OnFinish()
    {
        base.OnFinish();
        changer.BackOriginColor();
        progressImage.color = Color.gray;
        uiTimer.Stop(true);
    }
}
