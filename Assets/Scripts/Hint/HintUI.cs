using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HintUI : MonoBehaviour
{
    HintObejctCamera hintObejctCamera;
    CanvasGroup canvasGroup;

    public Text objectName;
    public Image image;

    bool state = false;
    bool stateLast = false;
    int tweenId;

    private void Start()
    {
        canvasGroup = GetComponentInChildren<CanvasGroup>();
        canvasGroup.alpha = 0;
    }

    private void Update()
    {
        if (state != stateLast)
        {
            if (state == true)
                DoFade(1f);
            if (state == false)
                DoFade(0f);

            stateLast = state;
        }
    }

    public void Show(HintObject obj)
    {
        objectName.text = obj.objectName;
        state = true;
    }

    public void Hide()
    {
        state = false;
    }

    void DoFade(float value)
    {
        if (DOTween.TweensById(tweenId) != null)
        {
            if (DOTween.TweensById(tweenId).Count != 0)
                DOTween.Kill(tweenId);
        }

        DOTween.To(() => canvasGroup.alpha, (f) => canvasGroup.alpha = f, value, .5f).SetId(tweenId);
    }
}