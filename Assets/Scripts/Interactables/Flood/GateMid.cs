using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateMid : InteracableObject
{
    public bool hasInstalled;
    [HideInInspector] public List<Transform> targets = new List<Transform>();
    public static int currentTarget = 0;

    protected override void Start()
    {
        base.Start();

        onReleaseEvent.AddListener(ResetCollider);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (hasInstalled) return;
        base.OnTriggerEnter(other);

        if (isGrabbing) return;

        if (other.gameObject == null) return;

        Transform t;
        if (targets.Contains(t = other.transform))
        {
            transform.SetPositionAndRotation(t.position, t.rotation);
            positionReset = false;
            Interactable = false;
            hasInstalled = true;

            targets[currentTarget].gameObject.SetActive(false);
            if (currentTarget < targets.Count - 1)
            {
                currentTarget++;
                targets[currentTarget].gameObject.SetActive(true);
            }
        }
    }
}
