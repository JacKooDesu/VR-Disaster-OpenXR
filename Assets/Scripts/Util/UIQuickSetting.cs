using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Cysharp.Threading.Tasks;

[RequireComponent(typeof(CanvasGroup))]
public class UIQuickSetting : MonoBehaviour
{
    public bool fadeIn;
    public bool fadeOut;

    CanvasGroup canvasGroup;
    bool originBlockRaycast;

    public bool bindSound = true;

    public bool hideAtStart = true;

    bool status;
    public bool Status
    {
        private set
        {
            if (originBlockRaycast)
                canvasGroup.blocksRaycasts = value;
            status = value;
        }
        get => status;
    }

    private void Start()
    {
        // canvasGroup = GetComponent<CanvasGroup>();
        // originBlockRaycast = canvasGroup.blocksRaycasts;
    }

    private void OnEnable()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        originBlockRaycast = canvasGroup.blocksRaycasts;

        if (bindSound)
            BindButton();

        if (hideAtStart)
        {
            canvasGroup.alpha = 0;
            Status = false;
        }
        else
        {
            Status = true;
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void TurnOn()
    {
        if (status)
            return;

        if (fadeIn)
        {
            FadingIn().Forget();
        }
    }

    public void TurnOff()
    {
        if (!status)
            return;
        if (fadeOut)
        {
            FadingOut().Forget();
        }
    }

    void BindButton()
    {
        Button[] tempButtons = GetComponentsInChildren<Button>();

        foreach (Button b in tempButtons)
        {
            // print(b.gameObject);
            GameHandler.Singleton.BindEvent(
                b.gameObject,
                EventTriggerType.PointerEnter,
                delegate
                {
                    JacDev.Audio.AudioHandler.Singleton.PlaySound(JacDev.Audio.AudioHandler.Singleton.soundList.select);
                    //GameHandler.Singleton.BlurCamera(true);
                });

            GameHandler.Singleton.BindEvent(
                b.gameObject,
                EventTriggerType.PointerDown,
                delegate
                {
                    JacDev.Audio.AudioHandler.Singleton.PlaySound(JacDev.Audio.AudioHandler.Singleton.soundList.hover);
                    //GameHandler.Singleton.BlurCamera(false);
                });

            // GameHandler.Singleton.BindEvent(
            //     b.gameObject,
            //     EventTriggerType.PointerExit,
            //     delegate { GameHandler.Singleton.BlurCamera(false); }
            // );
        }

        // if (GetComponent<Button>())
        // {
        //     GameHandler.Singleton.BindEvent(
        //         gameObject,
        //         EventTriggerType.PointerEnter,
        //         delegate
        //         {
        //             JacDev.Audio.AudioHandler.Singleton.PlaySound(JacDev.Audio.AudioHandler.Singleton.soundList.select);
        //         });

        //     GameHandler.Singleton.BindEvent(
        //         gameObject,
        //         EventTriggerType.PointerDown,
        //         delegate
        //         {
        //             JacDev.Audio.AudioHandler.Singleton.PlaySound(JacDev.Audio.AudioHandler.Singleton.soundList.hover);
        //         });
        // }

    }

    async UniTask FadingIn()
    {
        Status = true;
        while (Mathf.Abs(canvasGroup.alpha - 1) > 0.01f && Status)
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 1, .1f);
            await UniTask.Yield();
        }
        canvasGroup.alpha = 1;
    }

    async UniTask FadingOut()
    {
        Status = false;
        while (Mathf.Abs(canvasGroup.alpha - 0) > 0.01f && !Status)
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0, .1f);
            await UniTask.Yield();
        }
        canvasGroup.alpha = 0;
    }

    public async UniTask WaitStatusChange(UnityAction action, bool status)
    {
        while (Status != status)
        {
            await UniTask.Yield();
        }
        // Debug.Log("bool change");
        action.Invoke();
    }
}
