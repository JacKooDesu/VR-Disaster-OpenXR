using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class react : MonoBehaviour
{
    [SerializeField] GameViewEncoder _encoder;
    public GameObject ShowWarnObj;
    // private string StudentIP;
    // private int ScreenHeight, ScreenWidth;
    // private int IPShowCheck = 0;

    void Start()
    {
        FMNetworkManager.instance.OnReceivedStringDataEvent
            .AddListener(Action_ProcessStringData);
    }

    void OnDestroy()
    {
        FMNetworkManager.instance.OnReceivedStringDataEvent
            .RemoveListener(Action_ProcessStringData);
    }

    public void Action_ProcessStringData(string _string)
    {
        string[] sData = _string.Split(' ');
        var packetType = sData[0];

        // if (IPShowCheck == 0)
        // {
        //     IPShowCheck = 1;
        //     ShowIP.text = StudentIP;
        // }

        switch (packetType)
        {
            case "screen1":     // one on one
                AssignResize(GameViewResize.Quarter);
                SendCheck();
                _encoder.label = 1001;
                break;
            case "screen2":     // one on multi (multi mode, main)
                AssignResize(GameViewResize.Quarter);
                break;
            case "screen3":     // one on multi (multi mode, all)
                AssignResize(GameViewResize.OneEighth);
                break;
            case "screen5":     // one on multi (multi mode, side)
                AssignResize(GameViewResize.OneSixteenth);
                break;

            case "screen4":
                _encoder.label = int.Parse(sData[1]);
                break;

            case "Open1":
                ShowWarnObj?.SetActive(true);
                break;
            case "Close1":
                ShowWarnObj?.SetActive(false);
                break;
            case "Open2":
                ShowWarnObj?.SetActive(true);
                break;
            case "Close2":
                ShowWarnObj?.SetActive(false);
                break;

            default:
                break;
        }
    }

    void AssignResize(GameViewResize resize)
    {
        _encoder.Resize = resize;
    }

    void SendCheck()
    {
        string sendString = FMNetworkManager.instance.ReadLocalIPAddress + " " + "CheckScreen";
        FMNetworkManager.instance.SendToServer(sendString);
    }
}
