using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateSide : InteracableObject
{
    public bool hasInstalled;

    protected override void Start()
    {
        base.Start();

        onReleaseEvent.AddListener(() =>
        {
            ResetCollider();
        });
    }
}
