using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

public class TeleportPoint : MonoBehaviour
{
    public int materialColorIndex;
    public Color selectColor;
    public Color normalColor;
    [Range(0, 1)] public float colorLerpSpeed;

    public GameObject selectArrowParticle;

    bool isSelected = false;
    XRBaseRaycaster raycaster;

    // public System.Action onTeleportAction;
    public UnityEvent onSelectAction;
    public UnityEvent onTeleportAction;


    [Header("自訂屬性")]
    public string pointName;
    public bool showHint;

    bool invokeState = false; // 避免重複觸發
    TeleportationAnchor _anchorProxy;

    void Awake()
    {
        _anchorProxy = GetComponentInChildren<TeleportationAnchor>();

        if (_anchorProxy == null)
        {
            Debug.LogError("TeleportationAnchor component is missing on the TeleportPoint object.");
            return;
        }

        _anchorProxy.teleporting.AddListener(arg => onTeleportAction?.Invoke());
    }

    public void BeingSelect(XRBaseRaycaster raycaster)
    {
        if (isSelected)
            return;

        isSelected = true;
        onSelectAction.Invoke();
        this.raycaster = raycaster;
        CheckSelecting().Forget();

        if (showHint)
        {
            FindAnyObjectByType<HintCanvas>().SetHintText($"傳送至{pointName}", true);
        }
    }

    async UniTask CheckSelecting()
    {
        while (raycaster.HitResult.gameObject == gameObject)
        {
            await UniTask.Yield();
        }
        isSelected = false;
        FindAnyObjectByType<HintCanvas>().ShowHintText(false);
    }

    public async void BeingTeleported()
    {
        if (invokeState)
            return;

        print($"{gameObject.name} being teleport! {this.GetInstanceID()}");

        onTeleportAction.Invoke();

        // 避免重複觸發
        invokeState = true;
        await System.Threading.Tasks.Task.Yield();
        invokeState = false;
    }

    private void Update()
    {
        // LerpColor();

        // if (isSelected && !selectArrowParticle.activeInHierarchy)
        //     selectArrowParticle.SetActive(true);

        // if (!isSelected && selectArrowParticle.activeInHierarchy)
        //     selectArrowParticle.SetActive(false);

    }

    void LerpColor()
    {
        MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
        var m = mr.sharedMaterials[materialColorIndex];
        if (isSelected)
            m.color = Color.Lerp(m.color, selectColor, colorLerpSpeed);
        else
            m.color = Color.Lerp(m.color, normalColor, colorLerpSpeed);
    }
}
