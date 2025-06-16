using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PWave : Stage
{
    public Earthquake earthquake;

    public override void OnBegin()
    {
        base.OnBegin();
        FindObjectOfType<HintCanvas>().SetHintText("地震！地震！", true);
        StartCoroutine(MakeEarthquake());
    }

    public override void OnFinish()
    {
        //base.OnFinish();
        //GameHandler.Singleton.audioHandler.ClearSpeaker();
    }

    IEnumerator MakeEarthquake()
    {
        GameHandler.Singleton.player.SetCanMove(true);

        JacDev.Audio.Earthquake audio = (JacDev.Audio.Earthquake)GameHandler.Singleton.audioHandler;
        audio.ClearSpeaker();

        audio.PlayAudio(audio.cwbeew, true);    // 蜂鳴器

        yield return new WaitForSeconds(3f);

        audio.PlaySound(audio.PWave);   // 地震聲音
        audio.GetSoundAudioSource(audio.PWave).volume = .8f;
        earthquake.SetQuake(48f);

        yield return new WaitForSeconds(3f);

        audio.GetSpeakerAudioSource(audio.cwbeew).volume = .2f;
        audio.PlayAudio(audio.earthquakeRadio, false);      // 廣播

        yield return new WaitForSeconds(8f);

        audio.GetSoundAudioSource(audio.PWave).volume = .1f;
        audio.GetSpeakerAudioSource(audio.earthquakeRadio).volume = .1f;
        audio.GetSpeakerAudioSource(audio.cwbeew).volume = .05f;

        GameHandler.Singleton.StageFinish();

        while (earthquake.isQuaking)
            yield return null;

        audio.ClearSpeaker();
    }
}
