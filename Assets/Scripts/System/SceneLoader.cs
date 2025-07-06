using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour    // 場景載入腳本，不摧毀
{
    static SceneLoader singleton = null;

    public static SceneLoader Singleton
    {
        get
        {
            singleton = FindAnyObjectByType(typeof(SceneLoader)) as SceneLoader;

            if (singleton == null)
            {
                GameObject g = new GameObject("SceneLoader");
                singleton = g.AddComponent<SceneLoader>();
            }

            return singleton;
        }
    }

    //public float progress = 0f;

    public float fadeSpeed = .5f;

    public CanvasGroup BG;
    public Image loadArc;

    public string nameTemp;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        //Load("Earthquake");
    }

    public void Load(string scene)
    {
        GetComponent<Canvas>().worldCamera = GameHandler.Singleton.uiCamera;
        LoadSceneAsync(scene).Forget();
    }

    // 同步加載場景
    async UniTask LoadSceneAsync(string name)
    {
        float temp = 0;

        while (temp < 1)
        {
            temp += Time.deltaTime * fadeSpeed;
            BG.alpha = temp;
            await UniTask.Yield();
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(name);
        asyncLoad.allowSceneActivation = false;

        // float loadDamper = 0f;
        while (asyncLoad.progress < .9f)
        {
            loadArc.fillAmount = (1 / .9f) * asyncLoad.progress;

            await UniTask.Yield();
        }

        if (loadArc.fillAmount != 1)
        {
            loadArc.fillAmount = 1;
        }

        asyncLoad.allowSceneActivation = true;

        while (!asyncLoad.isDone)
            await UniTask.Yield();

        while (GameHandler.Singleton.uiCamera == null)
            await UniTask.Yield();

        print(GameHandler.Singleton.uiCamera);
        GetComponent<Canvas>().worldCamera = GameHandler.Singleton.uiCamera;

        while (temp > 0)
        {
            temp -= Time.deltaTime * fadeSpeed;
            BG.alpha = temp;
            await UniTask.Yield();
        }
    }

    public string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    public void SetName(string s)
    {
        nameTemp = s;
    }

    public string GetName()
    {
        if (nameTemp == null)
            return "none";
        else
            return nameTemp;
    }
}
