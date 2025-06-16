using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Glass : MonoBehaviour
{
    [HideInInspector] public GlassController glassController;

    public Transform origin;
    public Transform brokenParent;

    // public List<BrokenGlass> brokenGlasses = new List<BrokenGlass>();
    public InteracableObject[] breakPoints;

    public GameObject hint;

    public UnityEvent onGlassBrokenEvent;

    public void Setup()
    {
        EnableBreaker(false);
        BindBreakPoints();
        BindBrokenGlass();
    }

    public void EnableBreaker(bool enable)
    {
        foreach (var bPoint in breakPoints)
            bPoint.Interactable = enable;
    }

    void BindBreakPoints()
    {
        foreach (var bPoint in breakPoints)
        {
            bPoint.onHoverEvent.AddListener(GlassBroken);
        }
    }

    void BindBrokenGlass()
    {
        foreach (Transform brokenG in brokenParent)
        {
            var b = brokenG.gameObject.AddComponent<BrokenGlass>();
            b.glassController = glassController;
        }
    }

    void GlassBroken()
    {
        JacDev.Audio.AudioHandler audio = GameHandler.Singleton.audioHandler;
        audio.PlayAudio(audio.soundList.glassBreak, false, transform);

        origin.gameObject.SetActive(false);

        brokenParent.gameObject.SetActive(true);

        // 關閉提示及碰撞檢測
        glassController.ShowHint(false);
        glassController.EnableBreakers(false);

        onGlassBrokenEvent.Invoke();
    }
}
