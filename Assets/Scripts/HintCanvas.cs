using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CoroutineUtility;
using Cysharp.Threading.Tasks;

public class HintCanvas : MonoBehaviour
{
    public Transform head;
    bool headTracking = true;
    public bool HeadTracking
    {
        get => headTracking;
        set { headTracking = value; }
    }

    public float trackingDelay = 3f;
    float trackingDelayCounter = 0f;

    public float trackingAngle;

    public Text hintText;
    float hintTextShowingTimer;
    public GameObject currentActiveCanvas;
    List<GameObject> handlingCanvas = new List<GameObject>();

    Timer timer;

    private void Awake()
    {
        currentActiveCanvas = transform.GetChild(0).gameObject;

        timer = new Timer(0, () => { }, false);
    }

    public void SetHintText(string str, bool show, bool forceToForward = true, float time = 0f)
    {
        
        ShowHintText(show);
        SetHintText(str);

        timer.Stop();
        timer = new Timer(
            time == 0f ? str.Length * .5f : time,
            () => ShowHintText(false, false)
        );
    }

    public void ShowHintText(bool show, bool forceToForward = true)
    {
        hintText.gameObject.SetActive(show);

        if (!show)
            LerpToHeadAngle().Forget();

        if (forceToForward)
            LerpToHeadAngle().Forget();
    }

    public void SetHintText(string str)
    {
        hintText.text = str;
    }

    public GameObject AddCanvs(GameObject prefab)
    {
        var g = Instantiate(prefab, transform);
        handlingCanvas.Add(g);
        currentActiveCanvas = g;

        return g;
    }

    private void Update()
    {
        transform.position = new Vector3(head.position.x, transform.position.y, head.position.z);
        if (currentActiveCanvas == null)
        {
            var index = handlingCanvas.Count;
            handlingCanvas.RemoveAt(index - 1);
            currentActiveCanvas = handlingCanvas[index - 2];
        }

        CheckTracking();
    }

    #region Tracking
    void CheckTracking()
    {
        if (!headTracking)
            return;

        if (Mathf.Abs(Mathf.DeltaAngle(head.eulerAngles.y, transform.eulerAngles.y)) > trackingAngle)
            trackingDelayCounter += Time.timeScale != 0 ? Time.deltaTime : Time.unscaledDeltaTime;
        else
            trackingDelayCounter = 0f;


        if (trackingDelayCounter >= trackingDelay)
        {
            LerpToHeadAngle().Forget();
        }
    }

    public void ForceAlign()
    {
        trackingDelayCounter += trackingDelay;
    }

    async UniTask LerpToHeadAngle()
    {
        Vector3 origin = transform.eulerAngles;
        float targetAngleAmount = Mathf.DeltaAngle(transform.eulerAngles.y, head.eulerAngles.y);
        float yAngle = 0;
        while (Mathf.Abs(yAngle - targetAngleAmount) >= .001f)
        {
            await UniTask.Yield();
            yAngle = Mathf.Lerp(yAngle, targetAngleAmount, .1f);
            transform.eulerAngles = Vector3.up * yAngle + origin;

            trackingDelayCounter = 0f;

            await UniTask.Yield();
        }
    }
    #endregion
}
