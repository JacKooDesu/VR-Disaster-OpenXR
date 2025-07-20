using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeExtinguisher : Stage
{
    public Animation topExitAni;

    public InteracableObject fireExtinguisherBody;

    public MaterialChanger changer;

    public override void OnBegin()
    {
        base.OnBegin();
        changer.ChangeColor();

        topExitAni.Play();

        var player = GameHandler.Singleton.player;
        player.SetCanMove(true);

        JacDev.Audio.FireTruck audio = (JacDev.Audio.FireTruck)GameHandler.Singleton.audioHandler;

        audio.PlayAudio(audio.fireSound, true, stageObjects[0].obj.transform);
        audio.GetSpeakerAudioSource(audio.fireSound).volume = .5f;

        audio.PlaySound(audio.squatDown);
        // audio.currentPlayingSound = null;

        audio.PlayAudio(audio.bgm2, true, GameHandler.Singleton.player.transform).volume = .1f;
        fireExtinguisherBody.OnGrabbed.AddListener(ExtinguisherGrabbed);

        player.hintCanvas.SetHintText("照著箭頭引導指示拿取滅火器", true);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    public override void OnFinish()
    {
        base.OnFinish();
        changer.BackOriginColor();
        fireExtinguisherBody.OnGrabbed.RemoveListener(ExtinguisherGrabbed);

        fireExtinguisherBody.Interactable = false;
    }

    void ExtinguisherGrabbed()
    {
        isFinish = true;
    }
}
