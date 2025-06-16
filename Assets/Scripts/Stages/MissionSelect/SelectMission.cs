using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SelectMission : Stage
{
    public GameObject earthquake;
    public GameObject fireTruck;
    public GameObject flood;
    public GameObject leave;
    public ObjectSwitcher switcher;

    [Header("物件互動")]
    public Kit kit;
    public Plant plant;
    public InteracableObject hoverTest;

    public override void OnBegin()
    {
        #region  Earthquake
        GameHandler.Singleton.BindEvent(
            earthquake,
            EventTriggerType.PointerEnter,
            delegate
            {
                JacDev.Audio.TitleScene audio = (JacDev.Audio.TitleScene)GameHandler.Singleton.audioHandler;
                audio.PlaySound(audio.earthquakeIntro);
                switcher.Switch(0);
            }
        );

        GameHandler.Singleton.BindEvent(
            earthquake,
            EventTriggerType.PointerExit,
            delegate
            {
                JacDev.Audio.TitleScene audio = (JacDev.Audio.TitleScene)GameHandler.Singleton.audioHandler;
                Destroy(audio.GetSoundAudioSource(audio.earthquakeIntro).gameObject);
                switcher.HideAll();
            }
        );

        GameHandler.Singleton.BindEvent(
               earthquake,
               EventTriggerType.PointerDown,
               delegate
               {
                   JacDev.Audio.AudioHandler.Singleton.PlaySound(JacDev.Audio.AudioHandler.Singleton.soundList.hover);
                   GameHandler.Singleton.sceneLoader.LoadScene("Earthquake");
                   //GameHandler.Singleton.BlurCamera(false);
                   earthquake.GetComponent<EventTrigger>().enabled = false;
               });
        #endregion

        #region FireTruck
        GameHandler.Singleton.BindEvent(
            fireTruck,
            EventTriggerType.PointerEnter,
            delegate
            {
                JacDev.Audio.TitleScene audio = (JacDev.Audio.TitleScene)GameHandler.Singleton.audioHandler;
                audio.PlaySound(audio.fireTruckIntro);
                switcher.Switch(1);
            }
        );

        GameHandler.Singleton.BindEvent(
            fireTruck,
            EventTriggerType.PointerExit,
            delegate
            {
                JacDev.Audio.TitleScene audio = (JacDev.Audio.TitleScene)GameHandler.Singleton.audioHandler;
                Destroy(audio.GetSoundAudioSource(audio.fireTruckIntro).gameObject);
                switcher.HideAll();
            }
        );

        GameHandler.Singleton.BindEvent(
                fireTruck,
                EventTriggerType.PointerDown,
                delegate
                {
                    JacDev.Audio.AudioHandler.Singleton.PlaySound(JacDev.Audio.AudioHandler.Singleton.soundList.hover);
                    GameHandler.Singleton.sceneLoader.LoadScene("FireTruck");
                    fireTruck.GetComponent<EventTrigger>().enabled = false;
                    //GameHandler.Singleton.BlurCamera(false);
                });
        #endregion

        #region Flood
        GameHandler.Singleton.BindEvent(
            flood,
            EventTriggerType.PointerEnter,
            delegate
            {
                JacDev.Audio.TitleScene audio = (JacDev.Audio.TitleScene)GameHandler.Singleton.audioHandler;
                audio.PlaySound(audio.floodIntro);
                switcher.Switch(2);
            }
        );

        GameHandler.Singleton.BindEvent(
            flood,
            EventTriggerType.PointerExit,
            delegate
            {
                JacDev.Audio.TitleScene audio = (JacDev.Audio.TitleScene)GameHandler.Singleton.audioHandler;
                Destroy(audio.GetSoundAudioSource(audio.floodIntro).gameObject);
                switcher.HideAll();
            }
        );

        GameHandler.Singleton.BindEvent(
                flood,
                EventTriggerType.PointerDown,
                delegate
                {
                    JacDev.Audio.AudioHandler.Singleton.PlaySound(JacDev.Audio.AudioHandler.Singleton.soundList.hover);
                    GameHandler.Singleton.sceneLoader.LoadScene("Flood");
                    //GameHandler.Singleton.BlurCamera(false);
                    flood.GetComponent<EventTrigger>().enabled = false;
                });
        #endregion

        #region  Leave
        GameHandler.Singleton.BindEvent(
            leave,
            EventTriggerType.PointerEnter,
            delegate
            {
                JacDev.Audio.TitleScene audio = (JacDev.Audio.TitleScene)GameHandler.Singleton.audioHandler;
                audio.PlaySound(audio.soundList.select);
            }
        );

        GameHandler.Singleton.BindEvent(
            leave,
            EventTriggerType.PointerExit,
            delegate
            {
                JacDev.Audio.TitleScene audio = (JacDev.Audio.TitleScene)GameHandler.Singleton.audioHandler;
                Destroy(audio.GetSoundAudioSource(audio.earthquakeIntro).gameObject);
            }
        );

        GameHandler.Singleton.BindEvent(
               leave,
               EventTriggerType.PointerDown,
               delegate
               {
                   Application.Quit();
                   //GameHandler.Singleton.BlurCamera(false);
               });
        #endregion

        // CheckMission("Earthquake", earthquake);
        // CheckMission("FireTruck", fireTruck);
        // CheckMission("Flood", flood);

        #region Interact
        BindKit();
        BindPlant();
        BindHover();
        #endregion

        JacDev.Audio.TitleScene a = (JacDev.Audio.TitleScene)GameHandler.Singleton.audioHandler;
        a.PlayAudio(a.bgm, true, GameHandler.Singleton.player.transform).volume = .05f;
    }

    async void BindKit()
    {
        await System.Threading.Tasks.Task.Delay(500);
        kit.KitMissionSetup(4, i => BindKit(), 4, 2);
    }

    void BindPlant()
    {
        plant.onGrabEvent.AddListener(async () =>
        {
            var go = Instantiate(plant.gameObject, plant.transform.position, Quaternion.identity);
            var interact = go.GetComponent<Plant>();
            interact.Interactable = false;
            go.SetActive(false);
            // 待確認
            interact.GetComponent<Outline>().enabled = false;
            await System.Threading.Tasks.Task.Delay(500);
            interact.Interactable = true;
            go.SetActive(true);

            plant = interact;
            BindPlant();
        });

        // plant.onReleaseEvent.AddListener(async () =>
        // {
        //     await System.Threading.Tasks.Task.Delay(10000);
        //     Destroy(plant.gameObject);
        // });
    }

    void BindHover()
    {
        hoverTest.onHoverEvent.AddListener(async () =>
        {
            var mChanger = new MaterialChanger
            {
                parent = hoverTest.transform,
                targetColor = Color.red
            };
            mChanger.ChangeColor(2f);
            hoverTest.GetComponentInChildren<ParticleSystem>().Play();
            await System.Threading.Tasks.Task.Delay(2000);
            mChanger.BackOriginColor();
        });
    }

    public override void OnUpdate()
    {

    }

    public override void OnFinish()
    {

    }

    public void CheckMission(string name, GameObject objPanel)
    {
        if (GameHandler.playerData.GetMissionData(name) != null)
        {
            float t = GameHandler.playerData.GetMissionData(name).time;
            int m = Mathf.RoundToInt(t / 60);
            int s = Mathf.RoundToInt(t % 60);

            objPanel.GetComponentInChildren<Text>().text = m.ToString() + " : " + s.ToString();
            objPanel.transform.Find("Stamp").GetComponent<Image>().color = new Color(1f, 0.8078432f, 0f, 0.5f);
        }
        else
        {
            objPanel.GetComponentInChildren<Text>().text = "-- : --";
            objPanel.transform.Find("Stamp").GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);
        }
    }
}
