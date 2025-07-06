using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoverHandler : MonoBehaviour
{
    public Image hoverTimerUi;
    public bool mainMenuEnable;
    public float mainMenuTime = 5f;
    public bool debugMode = false;
    static bool isCallingMenu;  // 雙手合十，回主選單
    static float timer;     // 秒數回主選單

    public XRHandlerDeviceType DeviceType;
    private XRHandlerData handlerData;

    private void Start()
    {
        ResetImage();

        handlerData = XRInputManager.Instance.GetInputData<XRHandlerData>(DeviceType);
    }

    public void UpdateImage(float t)
    {
        hoverTimerUi.fillAmount = t;
    }

    public void ResetImage()
    {
        hoverTimerUi.fillAmount = 0;
    }

    private void Update()
    {
        if (handlerData.IsHandLostTracking || handlerData.IsHandNotFound)
            return;
            
        if (isCallingMenu)
        {
            timer += Time.deltaTime * .5f;
            UpdateImage(timer / mainMenuTime);

            if (timer >= mainMenuTime)
            {
                FindAnyObjectByType<AsyncLoadingScript>().LoadScene("MissionSelect");
                isCallingMenu = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!mainMenuEnable)
            return;

        if (isCallingMenu)
            return;

        if (Application.isEditor && !debugMode)
            return;

        if (other.gameObject.layer == gameObject.layer)
        {
            isCallingMenu = true;
            timer = 0f;

            foreach (var hh in FindObjectsOfType<HoverHandler>())
                hh.ResetImage();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!mainMenuEnable)
            return;

        if (!isCallingMenu)
            return;

        if (Application.isEditor && !debugMode)
            return;

        if (other.gameObject.layer == gameObject.layer)
        {
            isCallingMenu = false;
            timer = 0f;

            foreach (var hh in FindObjectsOfType<HoverHandler>())
                hh.ResetImage();
        }
    }
}
