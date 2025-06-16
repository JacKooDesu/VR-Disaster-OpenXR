using System;
using UnityEngine;
using UnityEngine.UI;

namespace Util
{
    public class UIMath
    {
        public static Vector2 WorldToCanvasPosition(Transform target, Camera cam, Canvas canvas)
        {
            Vector2 result;
            var rt = canvas.GetComponent<RectTransform>();
            var viewportPos = cam.WorldToViewportPoint(target.position);
            var proportionalPos = new Vector2(viewportPos.x * rt.sizeDelta.x, viewportPos.y * rt.sizeDelta.y);

            var offset = new Vector2((float)rt.sizeDelta.x / 2f, (float)rt.sizeDelta.y / 2f);

            result = proportionalPos - offset;

            return result;
        }
    }
}
