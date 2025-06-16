using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusCrack : Stage
{
    public float minTime;
    public float maxTime;

    public float crashTime = 5f;

    public Transform jointer;
    public Transform headTransform;

    bool hasTriggered = false;

    public MaterialChanger changer;

    public override void OnBegin()
    {
        GameHandler.Singleton.player.SetCanMove(false);
        headTransform = GameHandler.Singleton.player.head;
        changer.ChangeColor();
    }

    public override void OnFinish()
    {
    }

    public void WaitCrash()
    {
        if (hasTriggered)
            return;

        hasTriggered = true;
        changer.BackOriginColor();

        JacDev.Audio.FireTruck audio = (JacDev.Audio.FireTruck)GameHandler.Singleton.audioHandler;
        AudioSource a = audio.PlayAudio(audio.bgm1, true, GameHandler.Singleton.player.transform);
        a.volume = .2f;

        new CoroutineUtility.Timer(
            Random.Range(minTime, maxTime),
            () =>
            {
                Crash();
                Destroy(a.gameObject);
            }
        );
    }

    public void Crash()
    {
        Player p = GameHandler.Singleton.player;
        p.canRotate = false;
        jointer.parent.position = GameHandler.Singleton.player.head.position;
        Transform originParent = GameHandler.Singleton.player.head.parent;
        headTransform.SetParent(jointer);

        Rigidbody rb = jointer.GetComponent<Rigidbody>();
        rb.AddForce(jointer.forward * 5, ForceMode.Impulse);

        new CoroutineUtility.Timer(
            crashTime, () =>
            {
                headTransform.SetParent(originParent);
                jointer.gameObject.SetActive(false);
                //p.SetCanMove(true);

                GameHandler.Singleton.StageFinish();
                GameHandler.Singleton.player.canRotate = true;
            });
    }
}
