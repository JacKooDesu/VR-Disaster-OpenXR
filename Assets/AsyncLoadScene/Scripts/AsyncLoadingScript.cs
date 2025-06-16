using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AsyncLoadingScript : MonoBehaviour
{
    static string targetSceneName; // 目標跳轉場景
    // [SerializeField]
    // Slider slider;  // 讀條
    [SerializeField]
    Text text;  // 進度

    [SerializeField]
    Text loadingText;
    [SerializeField]
    Button finishButton;

    AsyncOperation async;   // 異步加載設定
    bool isLoading = false;

    public void LoadScene(string name)
    { // 傳入場景名稱
        if (isLoading)
            return;

        isLoading = true;
        targetSceneName = name;
        // SceneManager.LoadScene("AsyncLoadingScene");    // 跳到異步加載場景
        var fadeUtil = FindObjectOfType<CameraFadeUtil>();
        fadeUtil.FadeOut(.5f, null, () => SceneManager.LoadSceneAsync(targetSceneName));
        // async = SceneManager.LoadSceneAsync(targetSceneName);
        // async.allowSceneActivation = true;
        // StartCoroutine(Loading());
    }

    // private void Start()
    // {
    //     if (SceneManager.GetActiveScene().name == "AsyncLoadingScene")
    //     {
    //         async = SceneManager.LoadSceneAsync(targetSceneName);
    //         async.allowSceneActivation = false;
    //         StartCoroutine(Loading());
    //     }
    // }

    IEnumerator Loading()
    {
        float progress = 0f;
        // while (progress < 0.99f)
        // {
        //     progress += Mathf.Clamp(0, 0.01f, async.progress);
        //     slider.value = progress;
        //     text.text = Mathf.Floor(progress * 100f).ToString() + " %";
        //     yield return null;
        // }
        // loadingText.text = "載入中...";
        while (progress < 0.99f)
        {
            print(progress);
            progress = Mathf.Lerp(progress, async.progress / 9 * 10, Time.deltaTime);
            // slider.value = progress;
            // text.text = Mathf.Floor(progress * 100f).ToString() + " %";
            yield return null;
        }
        progress = 1f;
        // slider.value = 1f;
        // text.text = "100 %";
        // finishButton.gameObject.SetActive(true);
        // loadingText.text = "點擊繼續";
        // finishButton.onClick.AddListener(delegate { async.allowSceneActivation = true; });
        async.allowSceneActivation = true;

    }

    // public string GetCurrentSceneName()
    // {
    //     return SceneManager.GetActiveScene().name;
    // }

    public void LeaveGame()
    {
        Application.Quit();
    }
}
