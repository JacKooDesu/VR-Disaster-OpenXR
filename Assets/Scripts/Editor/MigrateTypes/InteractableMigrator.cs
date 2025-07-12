#if UNITY_EDITOR
using System;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Attachment;
using UnityEditor;

public partial class SceneMigrator
{
    static GenericMigrateType<InteracableObject, XRGrabInteractable> InteractableMigration()
    {
        Action<InteracableObject, XRGrabInteractable> migrationAction = (oldComponent, newComponent) =>
        {
            var go = oldComponent.gameObject;
            newComponent.farAttachMode = InteractableFarAttachMode.DeferToInteractor;
            ReflectAssign(oldComponent, "_interactableProxy", newComponent);
        };

        return new GenericMigrateType<InteracableObject, XRGrabInteractable>(migrationAction)
        {
            _targetFilter = x =>
                x.TryGetComponent<InteracableObject>(out _) &&
                !x.TryGetComponent<XRGrabInteractable>(out _)
        };
    }
}
#endif