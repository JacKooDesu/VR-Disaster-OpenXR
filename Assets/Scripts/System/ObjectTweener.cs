using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectTweener : MonoBehaviour  // 物件位移類別
{

    public Transform target;
    public int currentPoint = -1;

    [System.Serializable]
    public class TweenPoint
    {
        public iTween.EaseType easeType = iTween.EaseType.easeInOutSine;
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
        iTween.MoveTo(target.gameObject, points[currentPoint].position, moveTime);
        iTween.RotateTo(target.gameObject, points[currentPoint].eulerAngles, moveTime);
    }

    public void MoveToPoint(int p)  // 位移至定點
    {
        iTween.MoveTo(target.gameObject, points[p].position, moveTime);
        iTween.RotateTo(target.gameObject, points[p].eulerAngles, moveTime);

        currentPoint = p;
    }

    public void AddPoint()      // Editor mode function
    {

    }
}
