using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateMid : InteracableObject
{
    protected override void Start()
    {
        base.Start();

        onReleaseEvent.AddListener(ResetCollider);
    }
}
