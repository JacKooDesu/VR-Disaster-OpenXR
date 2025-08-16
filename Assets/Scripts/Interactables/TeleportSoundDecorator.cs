using JacDev.Audio;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

public class TeleportSoundDecorator : MonoBehaviour
{
    [SerializeField, SerializeReference]
    BaseTeleportationInteractable _tpInteractable;

    void Start()
    {
        if (_tpInteractable is null)
            return;

        _tpInteractable.teleporting.AddListener(
            _ => AudioHandler.Singleton.PlaySound(AudioHandler.Singleton.soundList.teleport));
    }
}
