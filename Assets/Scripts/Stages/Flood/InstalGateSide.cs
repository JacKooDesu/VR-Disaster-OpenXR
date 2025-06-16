using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstalGateSide : Stage
{
    public GameObject spotlight;
    public Transform targetParent;
    public Transform objParent;
    GateSide[] gates;
    public UIQuickSetting hint;

    public override void OnBegin()
    {
        base.OnBegin();
        spotlight.SetActive(true);
        targetParent.gameObject.SetActive(true);

        foreach (Transform t in objParent)
        {
            var interact = t.GetComponent<GateSide>();
            interact.Interactable = true;
        }

        gates = new GateSide[objParent.childCount];
        for (int i = 0; i < objParent.childCount; ++i)
            gates[i] = objParent.GetChild(i).GetComponent<GateSide>();

        JacDev.Audio.Flood a = (JacDev.Audio.Flood)GameHandler.Singleton.audioHandler;
        a.PlaySound(a.instalGateSide);

        onGetToTarget += () =>
        {
            new CoroutineUtility.Timer(3f, hint.TurnOn, null, hint.TurnOff);
            GameHandler.Singleton.player.hintCanvas.ForceAlign();
        };
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        foreach (var g in gates)
        {
            if (!g.hasInstalled)
                return;
        }
        isFinish = true;
    }

    public override void OnFinish()
    {
        base.OnFinish();
        targetParent.gameObject.SetActive(false);

        foreach (Transform t in objParent)
        {
            t.GetComponent<GateSide>().Interactable = false;
        }
    }
}
