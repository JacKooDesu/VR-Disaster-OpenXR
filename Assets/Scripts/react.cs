using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class react : MonoBehaviour
{
    [SerializeField] GameViewEncoder _encoder;
    public GameObject ShowWarnObj;
    // private string StudentIP;
    // private int ScreenHeight, ScreenWidth;
    // private int IPShowCheck = 0;

    CancellationTokenSource _checkConnectCt;

    void Start()
    {
        FMNetworkManager.instance.OnReceivedStringDataEvent
            .AddListener(Action_ProcessStringData);

        AssignResize(GameViewResize.One_32);

        _checkConnectCt = new();
        _checkConnectCt.RegisterRaiseCancelOnDestroy(gameObject);
        ConnectHeartbeat().Forget();

        async UniTask ConnectHeartbeat()
        {
            do
            {
                SendCheck();
            } while (!await UniTask.Delay(10000, cancellationToken: _checkConnectCt.Token).SuppressCancellationThrow());
        }
    }

    void OnDestroy()
    {
        FMNetworkManager.instance.OnReceivedStringDataEvent
            .RemoveListener(Action_ProcessStringData);
    }

    public void Action_ProcessStringData(string _string)
    {
        // means u r connected, no longer needed heartbeat
        _checkConnectCt?.Cancel(false);

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
                AssignResize(GameViewResize.Half);
                _encoder.label = 1001;
                break;
            case "screen2":     // one on multi (multi mode, main)
                AssignResize(GameViewResize.Half);
                break;
            case "screen3":     // one on multi (multi mode, all)
                AssignResize(GameViewResize.Quarter);
                break;
            case "screen5":     // one on multi (multi mode, side)
                AssignResize(GameViewResize.One_32);
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
