using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassController : MonoBehaviour
{
    [SerializeField] Glass[] glasses;

    int breakCount = 0;
    public int BreakCount
    {
        set
        {
            breakCount = value;
        }
        get
        {
            return breakCount;
        }
    }

    public LayerMask breakerLayer;

    private void OnEnable()
    {
        // breakerLayer = 1 << LayerMask.NameToLayer("Hover Check Layer");

        foreach (var g in glasses)
        {
            g.glassController = this;
            g.Setup();
        }

        ShowHint(false);
    }

    public void ShowHint(bool b){
        foreach(var g in glasses)
            g.hint.SetActive(b);
    }

    public void EnableBreakers(bool b){
        foreach(var g in glasses)
            g.EnableBreaker(b);
    }
}
