using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using DG.Tweening;
using System;

public class GameIntro : MonoBehaviour
{
    [SerializeField]
    GameObject _userDataInputUi;
    public UIQuickSetting[] uis;
    public float time = 3f;  // 時間
    public AudioSource introSE;

    private async UniTask Start()
    {
        await SetupUserData();

        introSE.PlayDelayed(time / 2);

        for (int i = 0; i < uis.Length; ++i)
        {
            uis[i].TurnOn();
            await UniTask.Delay(5000);
            uis[i].TurnOff();
            await UniTask.Delay(500);
        }

        var sceneLoader = FindAnyObjectByType<AsyncLoadingScript>();
        // var timer = new Timer(
        //     time,
        //     () => sceneLoader.LoadScene("MissionSelect")
        // );
        sceneLoader?.LoadScene("MissionSelect");
    }

    async UniTask SetupUserData()
    {
        var keyboard = _userDataInputUi.GetComponentInChildren<VirtualKeyboard>();
        var ui = _userDataInputUi.GetComponentInChildren<UIQuickSetting>();

        if (ui is not null)
            ui.TurnOn();
        else
            _userDataInputUi.SetActive(true);

        GameHandler.playerData = await keyboard.OnSubmit
            .OnInvokeAsync(this.GetCancellationTokenOnDestroy())
            .ContinueWith(userId => userId switch
            {
                _ when string.IsNullOrEmpty(userId) => new PlayerData("anonymous" + DateTime.Now.ToString("yyyyMMddHHmmss")),
                _ => GameHandler.Singleton.TryLoadPlayerData(userId, out var data) ? data : data
            });


        if (ui is not null)
            ui.TurnOff();
        else
            _userDataInputUi.SetActive(false);

        var cam = GameHandler.Singleton.cam;
        await DG.Tweening.DOVirtual
            .Color(
                cam.backgroundColor,
                Color.white,
                .5f,
                c => cam.backgroundColor = c)
            .Play()
            .AsyncWaitForCompletion();
    }
}
