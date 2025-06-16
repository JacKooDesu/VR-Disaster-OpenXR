using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(EventTrigger))]
public class MissionPanel : MonoBehaviour
{
    Image image;
    EventTrigger eventTrigger;
    public Color normal, hover, select;
    [Range(0f, 1f)]
    public float speed = .5f;
    int id;

    void Start()
    {
        id = gameObject.GetInstanceID();
        image = GetComponent<Image>();
        eventTrigger = GetComponent<EventTrigger>();

        EventBinder.Bind(eventTrigger, EventTriggerType.PointerEnter, (e) => ChangeColor(hover));
        EventBinder.Bind(eventTrigger, EventTriggerType.PointerExit, (e) => ChangeColor(normal));
    }

    void ChangeColor(Color c)
    {
        DOTween.Kill(id);
        image.DOColor(c, speed).SetId(id);
    }
}
