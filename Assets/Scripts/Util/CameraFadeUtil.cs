using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CameraFadeUtil : MonoBehaviour
{
    public Image image;
    int id;

    private void Start()
    {
        id = gameObject.GetInstanceID();
    }

    public void FadeOutIn(float t, System.Action onBegin = null, System.Action onBlack = null, System.Action onComplete = null)
    {
        if (onBegin != null)
            onBegin.Invoke();

        FadeOut(t, null, () => FadeIn(t, onBlack, onComplete));
    }

    public void FadeIn(float t, System.Action onBegin = null, System.Action onComplete = null)
    {
        if (onBegin != null) onBegin.Invoke();

        DOTween.Kill(id);

        image.DOFade(0f, t).OnComplete(
            () =>
            {
                if (onComplete != null)
                    onComplete.Invoke();
            }
        ).SetId(id);
    }

    public void FadeOut(float t, System.Action onBegin = null, System.Action onComplete = null)
    {
        if (onBegin != null) onBegin.Invoke();

        DOTween.Kill(id);

        image.DOFade(1f, t).OnComplete(
            () =>
            {
                if (onComplete != null)
                    onComplete.Invoke();
            }
        ).SetId(id);
    }
}
