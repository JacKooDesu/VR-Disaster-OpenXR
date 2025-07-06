using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[System.Serializable]
public class ObjectTweener : MonoBehaviour  // 物件位移類別
{

    public Transform target;
    public int currentPoint = -1;

    [System.Serializable]
    public class TweenPoint
    {
        public DG.Tweening.Ease easeType = DG.Tweening.Ease.InOutSine;
        public float animationTime = .8f;

    }

    public Transform[] points;

    public float moveTime = .8f;    // 之後連同位移點寫入 Class或 Struct

    public void SetTarget(Transform t)  // 綁定位移物件
    {
        target = t;
    }

    public void MoveNextPoint()     // 尚未定義完整
    {
        currentPoint++;
        DOTween.To(() => target.position, x => target.position = x, points[currentPoint].position, moveTime);
        DOTween.To(() => target.eulerAngles, x => target.eulerAngles = x, points[currentPoint].eulerAngles, moveTime);
    }

    public void MoveToPoint(int p)  // 位移至定點
    {
        DOTween.To(() => target.position, x => target.position = x, points[p].position, moveTime);
        DOTween.To(() => target.eulerAngles, x => target.eulerAngles = x, points[p].eulerAngles, moveTime);

        currentPoint = p;
    }

    public void AddPoint()      // Editor mode function
    {

    }
}
