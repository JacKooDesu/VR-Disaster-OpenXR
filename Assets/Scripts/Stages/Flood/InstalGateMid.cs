using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Runtime.Common;

public class InstalGateMid : Stage
{
    public GameObject spotlight;
    public Transform objParent;
    [SerializeField] 
    List<Transform> targets = new List<Transform>();

    public override void OnBegin()
    {
        JacDev.Audio.Flood a = (JacDev.Audio.Flood)GameHandler.Singleton.audioHandler;

        foreach (Transform t in objParent)
        {
            var interact = t.GetComponent<GateMid>();
            interact.Interactable = true;

            var trigger = t.gameObject.AddComponent<TriggerEventHandler>();
            trigger.Register<NearCollisionFlag>(
                TriggerEventHandler.Timing.Stay,
                col =>
                {
                    if (!interact.Interactable || interact.IsGrabbing)
                        return;

                    var colTransform = col.transform;
                    if (!targets.Contains(colTransform))
                        return;

                    var t = targets[0];
                    interact.transform.SetPositionAndRotation(t.position, t.rotation);
                    interact.positionReset = false;
                    interact.Interactable = false;
                    t.gameObject.SetActive(false);
                    trigger.enabled = false;

                    var snd = a.PlayAudio(a.gateInstall, false, interact.transform);
                    snd.transform.SetParent(null);

                    targets.RemoveAt(0);
                    if (targets.Count > 0)
                        targets[0].gameObject.SetActive(true);
                    else
                        isFinish = true;
                });
        }

        a.PlaySound(a.instalGateMid);

        targets[0].gameObject.SetActive(true);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
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
