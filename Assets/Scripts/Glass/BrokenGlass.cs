using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class BrokenGlass : MonoBehaviour
{
    public Material originMaterial;
    public Material hintMaterial;

    public GlassController glassController;

    bool hasHit;

    private void Hit()
    {
        JacDev.Audio.AudioHandler audio = GameHandler.Singleton.audioHandler;
        audio.PlayAudio(audio.soundList.glassBreak, false, transform);

        var rig = GetComponent<Rigidbody>();
        rig.isKinematic = false;
        rig.AddForce(transform.forward, ForceMode.Impulse);

        glassController.BreakCount++;

        var timer = new CoroutineUtility.Timer(3f, () => Destroy(gameObject));
        hasHit = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit)
            return;
        // print($"{other.gameObject.layer} , {glassController.breakerLayer}");
        // if (other.gameObject.layer != glassController.breakerLayer.value)
        //     return;
        Hit();
    }
}
