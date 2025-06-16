using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoroutineUtility;
using System.Threading.Tasks;

public class GameIntro : MonoBehaviour
{
    public UIQuickSetting[] uis;
    public float time = 3f;  // 時間
    public AudioSource introSE;

    private async void Start()
    {
        introSE.PlayDelayed(time / 2);

        for (int i = 0; i < uis.Length; ++i)
        {
            uis[i].TurnOn();
            await Task.Delay(5000);
            uis[i].TurnOff();
            await Task.Delay(500);
        }

        var sceneLoader = FindObjectOfType<AsyncLoadingScript>();
        // var timer = new Timer(
        //     time,
        //     () => sceneLoader.LoadScene("MissionSelect")
        // );
        sceneLoader?.LoadScene("MissionSelect");
    }
}
