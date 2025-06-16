using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitItem : InteracableObject
{
    [Header("icon")]
    public Sprite icon;
    [Header("內部設定")]
    public bool isCorrect = true;
    public bool hasTaken = false;
    public bool inPack = false;

    protected override void Start()
    {
        base.Start();
        onGrabEvent.AddListener(() => hasTaken = true);
        onReleaseEvent.AddListener(() => { new CoroutineUtility.Timer(3f, () => ResetPosition()); });
    }

    protected override void Update()
    {
        base.Update();
        if (!isGrabbing)
            transform.Rotate(Vector3.up * 20 * Time.deltaTime);
    }
}
