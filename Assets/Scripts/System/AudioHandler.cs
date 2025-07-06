using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace JacDev.Audio
{
    public class AudioHandler : MonoBehaviour   // 寫得極爛，管所有音源
    {
        static AudioHandler singleton = null;
        public static AudioHandler Singleton
        {
            get
            {
                singleton = FindAnyObjectByType(typeof(AudioHandler)) as AudioHandler;

                if (singleton == null)
                {
                    GameObject g = new GameObject("AudioHandler");
                    singleton = g.AddComponent<AudioHandler>();
                }

                return singleton;
            }
        }

        public GameObject audioSourcePrefab;

        public GameObject speaker;  // 場景喇叭
        public GameObject speakerPrefab;

        [Header("Sound List")]
        public SoundList soundList;

        public AudioSource currentPlayingSound;

        public AudioSource PlaySound(AudioClip sound)  // 播音效，從玩家身上2D
        {
            GameObject temp = Instantiate(audioSourcePrefab, GameHandler.Singleton.player.transform);
            AudioSource audioSource = temp.GetComponent<AudioSource>();
            audioSource.clip = sound;
            audioSource.Play();
            DestroyAudioSource(sound.length, temp).Forget();

            return currentPlayingSound = audioSource;
        }

        public void PlaySounds(AudioClip[] sounds, float interval = .5f)
        {
            float t = 0f;
            foreach (AudioClip a in sounds)
            {
                GameHandler.Singleton.Counter(t, t, () => PlaySound(a)).Forget();
                t += a.length + interval;
            }
        }

        public AudioSource PlayAudio(AudioClip audio, bool loop)
        {
            GameObject temp = Instantiate(speakerPrefab, speaker.transform);
            AudioSource audioSource = temp.GetComponent<AudioSource>();
            audioSource.clip = audio;

            audioSource.Play();

            if (loop)
                audioSource.loop = loop;
            else
                DestroyAudioSource(audio.length, temp).Forget();

            return audioSource;
        }

        public AudioSource PlayAudio(AudioClip audio, bool loop, Transform target)
        {
            if (target.GetComponentInChildren<AudioSource>())
                if (target.GetComponentInChildren<AudioSource>().clip == audio)
                    return target.GetComponentInChildren<AudioSource>();

            GameObject temp = Instantiate(speakerPrefab, target);
            temp.transform.localPosition = Vector3.zero;

            AudioSource audioSource = temp.GetComponent<AudioSource>();
            audioSource.clip = audio;

            audioSource.Play();

            if (loop)
                audioSource.loop = loop;
            else
                DestroyAudioSource(audio.length, temp).Forget();

            return audioSource;
        }

        // 摧毀音源
        async UniTask DestroyAudioSource(float t, GameObject target)
        {
            await UniTask.WaitForSeconds(t);
            Destroy(target);
        }

        // 清空喇叭
        public void ClearSpeaker()
        {
            for (int i = speaker.transform.childCount; i > 0; --i)
            {
                Destroy(speaker.transform.GetChild(i - 1).gameObject);
            }
        }

        public void StopAll()
        {
            foreach (AudioSource a in FindObjectsByType<AudioSource>(FindObjectsSortMode.None))
            {
                a.Stop();
            }
        }

        public void StopCurrent(){
            if(currentPlayingSound != null){
                currentPlayingSound.Stop();
            }
        }

        public void Windup(float f){
            if(currentPlayingSound != null){
                currentPlayingSound.pitch = f;
            }
        }

        public AudioSource GetSpeakerAudioSource(AudioClip ac)
        {
            // for (int i = speaker.transform.childCount; i > 0; --i)
            // {
            //     if (speaker.transform.GetChild(i - 1).GetComponent<AudioSource>().clip == ac)
            //         return speaker.transform.GetChild(i - 1).GetComponent<AudioSource>();
            // }

            foreach (AudioSource a in FindObjectsByType<AudioSource>(FindObjectsSortMode.None))
            {
                if (a.clip == ac)
                    return a;
            }

            return null;
        }

        public AudioSource GetSoundAudioSource(AudioClip ac)
        {
            for (int i = GameHandler.Singleton.player.transform.childCount; i > 0; --i)
            {
                if (GameHandler.Singleton.player.transform.GetChild(i - 1).GetComponent<AudioSource>().clip == ac)
                    return GameHandler.Singleton.player.transform.GetChild(i - 1).GetComponent<AudioSource>();
            }

            return null;
        }
    }
}
