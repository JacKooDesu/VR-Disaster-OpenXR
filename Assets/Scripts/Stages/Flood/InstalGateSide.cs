using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Runtime.Common;
using UnityEngine;

public class InstalGateSide : Stage
{
    public GameObject spotlight;
    public Transform targetParent;
    public Transform objParent;
    GateSide[] gates;
    public UIQuickSetting hint;
    [SerializeField]
    Material _sideMat;
    List<NearCollisionFlag> _targets;

    public override void OnBegin()
    {
        base.OnBegin();
        spotlight.SetActive(true);
        targetParent.gameObject.SetActive(true);
        _targets = targetParent.GetComponentsInChildren<NearCollisionFlag>().ToList();

        JacDev.Audio.Flood a = (JacDev.Audio.Flood)GameHandler.Singleton.audioHandler;

        foreach (Transform t in objParent)
        {
            var interact = t.GetComponent<GateSide>();
            interact.Interactable = true;
        }

        gates = new GateSide[objParent.childCount];
        for (int i = 0; i < objParent.childCount; ++i)
        {
            var gate = objParent.GetChild(i).GetComponent<GateSide>();
            var trigger = gate.gameObject.AddComponent<TriggerEventHandler>();
            trigger.Register<NearCollisionFlag>(
                TriggerEventHandler.Timing.Stay,
                col =>
                {
                    if (gate.IsGrabbing)
                        return;

                    var colTransform = col.transform;
                    if (!_targets.Remove(col))
                        return;

                    var snd = a.PlayAudio(a.gateInstallComplete, false, gate.transform);
                    snd.transform.SetParent(null);

                    gate.hasInstalled = true;
                    gate.gameObject.SetActive(false);
                    colTransform.GetComponent<MeshRenderer>().SetMaterials(new() { _sideMat });
                }
            );

            gates[i] = gate;
        }

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
    }
}
