using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class PWave : Stage
{
    public Earthquake earthquake;

    public override void OnBegin()
    {
        base.OnBegin();
        FindObjectOfType<HintCanvas>().SetHintText("地震！地震！", true);
        MakeEarthquake().Forget();
    }

    public override void OnFinish()
    {
        //base.OnFinish();
        //GameHandler.Singleton.audioHandler.ClearSpeaker();
    }

    async UniTask MakeEarthquake()
    {
        GameHandler.Singleton.player.SetCanMove(true);

        JacDev.Audio.Earthquake audio = (JacDev.Audio.Earthquake)GameHandler.Singleton.audioHandler;
        audio.ClearSpeaker();

        audio.PlayAudio(audio.cwbeew, true);    // 蜂鳴器

        await UniTask.Delay(3000);

        audio.PlaySound(audio.PWave);   // 地震聲音
        audio.GetSoundAudioSource(audio.PWave).volume = .8f;
        earthquake.SetQuake(48f);

        await UniTask.Delay(3000);

        audio.GetSpeakerAudioSource(audio.cwbeew).volume = .2f;
        audio.PlayAudio(audio.earthquakeRadio, false);      // 廣播

        await UniTask.Delay(8000);

        audio.GetSoundAudioSource(audio.PWave).volume = .1f;
        audio.GetSpeakerAudioSource(audio.earthquakeRadio).volume = .1f;
        audio.GetSpeakerAudioSource(audio.cwbeew).volume = .05f;

        GameHandler.Singleton.StageFinish();

        while (earthquake.isQuaking)
            await UniTask.Yield();

        audio.ClearSpeaker();
    }
}
