﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using JacDev.Audio;
using Cysharp.Threading.Tasks;

public class GameHandler : MonoBehaviour
{
    static GameHandler singleton = null;

    public static GameHandler Singleton
    {
        get
        {
            singleton = FindAnyObjectByType(typeof(GameHandler)) as GameHandler;

            if (singleton == null)
            {
                GameObject g = new GameObject("GameHandler");
                singleton = g.AddComponent<GameHandler>();
            }

            return singleton;
        }
    }

    [Header("Player")]
    public Player player;   // 玩家

    [Header("Stage")]
    public Stage firstStage;    // 首 Stage
    // public Stage finalStage;    // 末 Stage

    [Header("Cameras")]
    public Camera cam;  // 相機
    public Camera uiCamera; // 專門渲染UI的相機

    public AudioHandler audioHandler;
    //public GameObject curveLineRendererPrefab;  // 曲線渲染預製物

    [SerializeField] Stage currentStage; // 當前 Stage

    public Transform ObjectParent;

    public AsyncLoadingScript sceneLoader;

    float timer = 0f;

    int currentMissionIndex;
    public MissionSetting missionSetting;
    public static PlayerData playerData;   // 玩家資料序列化
    public List<Stage> allStg = new List<Stage>();

    private void Start()
    {
        if (playerData == null)
            playerData = new PlayerData();

        if (player == null)
            player = FindAnyObjectByType<Player>();

        var currentScene = SceneLoader.Singleton.GetCurrentSceneName();
        if (currentScene != "MissionSelect" && currentScene != "Result Scene")
        {
            playerData.SetMissionData(currentScene);
            currentMissionIndex = 0;

            PlayStage(firstStage).Forget();
        }
        else
        {
            firstStage.OnBegin();
        }
    }

    public async UniTask PlayStage(Stage stg)
    {
        print(stg.name);

        currentMissionIndex = allStg.IndexOf(stg);

        timer = 0;
        var stgSetting = missionSetting.settings[currentMissionIndex];
        var stgData = new PlayerData.MissionData.StgData();

        currentStage = stg;
        currentStage.score = stgSetting.score;
        stgData.stgName = stgSetting.name;

        stg.OnBegin();

        while (!stg.isFinish)
        {
            timer += Time.deltaTime;
            stg.OnUpdate();
            await UniTask.Yield();
        }

        stg.OnFinish();

        StopCoroutine("Counter");

        stgData.score = currentStage.score;
        playerData.SetStageData(stgData);
        stgData.time = timer;

        if (stg.nextStage != null)
        {
            audioHandler.StopCurrent();
            stg.StopAllCoroutines();

            await PlayStage(stg.nextStage);
        }
        else
        {
            while (audioHandler.currentPlayingSound != null)
                await UniTask.Yield();

            // SavePlayerData();
            sceneLoader.LoadScene("Result Scene");
        }
    }

    // 當前 Stage完成
    [ContextMenu("強制完成關卡")]
    public void StageFinish()
    {
        currentStage.isFinish = true;
    }

    // 綁Event在物件上
    public void BindEvent(GameObject g, EventTriggerType type, UnityAction<BaseEventData> call)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener(call);

        EventTrigger tempTrigger;
        if (g.GetComponent<EventTrigger>())
            tempTrigger = g.GetComponent<EventTrigger>();
        else
            tempTrigger = g.AddComponent<EventTrigger>();

        tempTrigger.triggers.Add(entry);
    }

    /*
        public void GrayCamera(bool status)
        {
            Grayscale grayscale;
            if (cam.GetComponent<Grayscale>())
                grayscale = cam.GetComponent<Grayscale>();
            else
                grayscale = cam.gameObject.AddComponent<Grayscale>();

            grayscale.enabled = status;
        }
        */

    public async UniTask Counter(float min, float max, UniTask nextFunction)
    {
        float t = Random.Range(min, max);
        float counter = 0;

        while (counter < t)
        {
            counter += Time.deltaTime;

            await UniTask.Yield();
        }

        await nextFunction;
    }

    public async UniTask Counter(float min, float max, UnityAction action)
    {
        float t = Random.Range(min, max);
        float counter = 0;

        while (counter < t)
        {
            counter += Time.deltaTime;

            await UniTask.Yield();
        }

        action.Invoke();
    }

    public async UniTask Counter(float t, UnityAction action)
    {
        float counter = 0;
        while (counter < t)
        {
            counter += Time.deltaTime;
            await UniTask.Yield();
        }

        action.Invoke();
    }

    public void MovePlayer(Transform t)
    {
        player.transform.position = t.position;
        player.transform.rotation = t.rotation;
    }

    public void SavePlayerData()
    {
        FileManager<PlayerData>.Save($"{playerData.stuID}_Data", playerData, "PlayerData");
        print("Save");
    }

    public void LoadPlayerData(string name)
    {
        FileManager<PlayerData>.Load("PlayerData", $"{name}_Data", playerData);
    }

    public void SetPlayerData(PlayerData d)
    {
        playerData = d;
    }

    public void SetPlayerName(Text text)
    {
        playerData.stuID = text.text;
    }

    public void SetPlayerName(string text)
    {
        playerData.stuID = text;
        SceneLoader.Singleton.SetName(text);
    }

    public Stage GetCurrentStage()
    {
        return currentStage;
    }

    public void LeaveGame()
    {
        Application.Quit();
    }
}
