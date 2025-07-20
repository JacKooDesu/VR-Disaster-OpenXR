using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoroutineUtility;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

public class OutBus : Stage
{
    public GameObject finishHint;
    Timer uiTimer;
    [SerializeField] TeleportationAnchor _tpAnchor;
    public override void OnBegin()
    {
        base.OnBegin();

        _tpAnchor.teleporting.AddListener(_ =>
        {
            _tpAnchor.gameObject.SetActive(false);
            JacDev.Audio.FireTruck audio = (JacDev.Audio.FireTruck)GameHandler.Singleton.audioHandler;
            audio.PlaySound(audio.finish);

            uiTimer = new Timer(
                audio.finish.length,
                () => finishHint.SetActive(true),
                (f) => { },
                () => isFinish = true);
        });
    }

    public override void OnFinish()
    {
        base.OnFinish();
        finishHint.SetActive(false);
    }
}
