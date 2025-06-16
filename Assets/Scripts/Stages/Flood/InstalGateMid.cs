using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class InstalGateMid : Stage
{
    public GameObject spotlight;
    public Transform objParent;
    GateMid[] gates;
    [SerializeField] List<Transform> targets = new List<Transform>();

    public override void OnBegin()
    {
        foreach (Transform t in objParent)
        {
            var interact = t.GetComponent<GateMid>();
            interact.Interactable = true;
            interact.targets = targets;
        }

        JacDev.Audio.Flood a = (JacDev.Audio.Flood)GameHandler.Singleton.audioHandler;
        a.PlaySound(a.instalGateMid);

        targets[0].gameObject.SetActive(true);

        gates = new GateMid[objParent.childCount];
        for (int i = 0; i < objParent.childCount; ++i)
            gates[i] = objParent.GetChild(i).GetComponent<GateMid>();
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
        foreach (Transform t in objParent)
        {
            t.GetComponent<GateMid>().Interactable = false;
        }

        RotateAnimation();

        spotlight.SetActive(false);
    }

    void RotateAnimation()
    {
        foreach (var ui in objParent.GetComponentsInChildren<UIQuickSetting>())
        {
            ui.TurnOn();
            ui.transform.DORotate(Vector3.back * 360, 3f, RotateMode.LocalAxisAdd).OnComplete(
                () => ui.TurnOff()
            );
        }
    }
}
