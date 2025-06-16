using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class Plant : InteracableObject
{
    public bool hasMoved = false;
    public bool isBroken = false;
    public ObjectSwitcher switcher;

    Vector3 lastVelocity;

    private void OnCollisionEnter(Collision other)
    {
        if (isGrabbing)
            return;

        if (other.gameObject.layer != 0)
            return;

        if (lastVelocity.magnitude >= 3)
        {
            switcher.Switch(1);
            isBroken = true;
        }
        else
        {
            isBroken = false;
        }

        hasMoved = true;

        rig.isKinematic = true;
        GetComponent<Collider>().enabled = false;
        GetComponent<Outline>().enabled = false;

        this.enabled = false;
    }

    private void LateUpdate()
    {
        lastVelocity = rig.linearVelocity;
    }
}
