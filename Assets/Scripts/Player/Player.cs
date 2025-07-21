using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using DG.Tweening;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Attachment;
using System;
using System.Reflection;
using Cysharp.Threading.Tasks;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using Unity.Mathematics;

public class Player : MonoBehaviour
{
    public Transform head;

    public bool isStop = true;

    float counter = 0;
    float stopTime = 3f;

    Rigidbody rb;

    public HintCanvas hintCanvas;
    public CameraFadeUtil fadeUtil;

    public bool hasTarget;  //是否有目標物

    public bool canMove = true;
    public bool canRotate = true;
    public float moveDistance = 100f;
    public bool isTeleport = false;
    public Vector3 teleportTarget;

    float originHeight;

    public Kit kit;

    [Header("控制器")]
    public Transform leftHandler;
    public Transform rightHandler;

    [SerializeField]
    float _objectToHandTime = .2f;

    public Transform foot;

    // Overlay Effect 設定
    UnityStandardAssets.ImageEffects.ScreenOverlay[] overlays;
    static float overlayOriginValue;

    [Header("Navigator")]
    public NavMeshAgent agent;
    public LineRendererUtil line;

    public UnityEngine.Events.UnityEvent onTeleportEvent;

    [Header("提示UI")]
    [SerializeField] UIQuickSetting warningUi;
    [SerializeField] UIQuickSetting nguUi;   //never give up

    ControllerInputActionManager _leftController;
    ControllerInputActionManager _rightController;
    NearFarInteractor _leftNearFarInteractor;
    NearFarInteractor _rightNearFarInteractor;
    InteractionAttachController _leftInteractionAttachController;
    InteractionAttachController _rightInteractionAttachController;

    int _originalTeleportLayer;
    XRRayInteractor _leftTeleportInteractor;
    XRRayInteractor _rightTeleportInteractor;

    public float State_LTrigger => _leftNearFarInteractor?.selectInput.ReadValue() ?? 0f;
    public float State_RTrigger => _rightNearFarInteractor?.selectInput.ReadValue() ?? 0f;
    public bool State_LGrip => _leftNearFarInteractor?.activateInput.ReadIsPerformed() ?? false;
    public bool State_RGrip => _rightNearFarInteractor?.activateInput.ReadIsPerformed() ?? false;

    InteracableObject _leftObject;
    InteracableObject _rightObject;

    public bool State_LInteracting => _leftObject != null;
    public bool State_RInteracting => _rightObject != null;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        _leftController = leftHandler.GetComponentInChildren<ControllerInputActionManager>();
        _rightController = rightHandler.GetComponentInChildren<ControllerInputActionManager>();

        _leftNearFarInteractor = leftHandler.GetComponentInChildren<NearFarInteractor>();
        _rightNearFarInteractor = rightHandler.GetComponentInChildren<NearFarInteractor>();

        _leftInteractionAttachController = _leftNearFarInteractor.interactionAttachController as InteractionAttachController;
        _rightInteractionAttachController = _rightNearFarInteractor.interactionAttachController as InteractionAttachController;

        _leftTeleportInteractor = leftHandler.GetComponentInChildren<XRRayInteractor>(true);
        _rightTeleportInteractor = rightHandler.GetComponentInChildren<XRRayInteractor>(true);

        _originalTeleportLayer = _leftTeleportInteractor.interactionLayers.value;
    }

    private async void Start()
    {
        // hintCanvas.head = head;

        // curveLine.gameObject.SetActive(false);
        originHeight = transform.position.y;

        if (GetComponentInChildren<NavMeshAgent>() != null)
        {
            RaycastHit hit;
            Physics.Raycast(transform.position, -transform.up, out hit);
            NavMeshAgent agent = GetComponentInChildren<NavMeshAgent>();
            agent.baseOffset = Vector3.Distance(transform.position, hit.point);
        }

        // SetupOverlayEffect();

        onTeleportEvent.AddListener(() => Debug.Log(agent.Warp(transform.position)));

        // 避免過快載入(下下策)
        await System.Threading.Tasks.Task.Delay(800);
        fadeUtil.FadeIn(.5f);

        _leftNearFarInteractor.selectEntered.AddListener(args =>
            TryUpdateSelectEnter(args, _leftInteractionAttachController, ref _leftObject));
        _rightNearFarInteractor.selectEntered.AddListener(args =>
            TryUpdateSelectEnter(args, _rightInteractionAttachController, ref _rightObject));
        _leftNearFarInteractor.selectExited.AddListener(args =>
            TryUpdateSelectExit(args, ref _leftObject));
        _rightNearFarInteractor.selectExited.AddListener(args =>
            TryUpdateSelectExit(args, ref _rightObject));

        void TryUpdateSelectEnter(
            SelectEnterEventArgs args, InteractionAttachController attachController, ref InteracableObject obj)
        {
            obj = args.interactableObject.transform
                .GetComponent<InteracableObject>();

            if (obj is null)
                return;

            obj?.UpdateGrabState(
                attachController.hasOffset ?
                    InteracableObject.EGrabMode.Fishing :
                    InteracableObject.EGrabMode.Grabbed);
        }

        void TryUpdateSelectExit(
            SelectExitEventArgs args, ref InteracableObject obj)
        {
            if (obj is null)
                return;

            obj?.Released();

            obj = null;
        }
    }

    void Update()
    {
        CheckPullToHand(_leftObject, _leftNearFarInteractor.activateInput, _leftInteractionAttachController);
        CheckPullToHand(_rightObject, _rightNearFarInteractor.activateInput, _rightInteractionAttachController);
    }

    void CheckPullToHand(
        InteracableObject target,
        XRInputButtonReader input,
        InteractionAttachController attachController)
    {
        if (target is null || !input.ReadIsPerformed())
            return;

        if (!target.canGrab ||
            target.GrabMode is not InteracableObject.EGrabMode.Fishing)
            return;

        PullAnimation().Forget();

        async UniTask PullAnimation()
        {
            IInteractionAttachController controller = attachController as IInteractionAttachController;

            var anchor = controller.GetOrCreateAnchorTransform();
            var anchorParent = anchor.parent;

            var vec = anchor.position;
            var endpoint = anchorParent.position;

            target.UpdateGrabState(InteracableObject.EGrabMode.Pulling);

            var tween = DOTween.To(
                    () => vec,
                    x => vec = x,
                    endpoint,
                    _objectToHandTime).Play();

            while (tween.IsPlaying() && controller.hasOffset)
            {
                controller.MoveTo(vec);
                await UniTask.Yield();
            }

            controller.ResetOffset();
            target.UpdateGrabState(InteracableObject.EGrabMode.Grabbed);
        }
    }

    void SetupOverlayEffect()
    {
        overlays = head.GetComponentsInChildren<UnityStandardAssets.ImageEffects.ScreenOverlay>();
        overlayOriginValue = overlays[0].intensity;
    }

    public void SetCanMove(bool b)
    {
        // if (rb == null)
        //     rb = GetComponent<Rigidbody>();

        if (b)
        {
            _leftTeleportInteractor.interactionLayers
                = _rightTeleportInteractor.interactionLayers
                = _originalTeleportLayer;
        }
        else
        {
            _leftTeleportInteractor.interactionLayers
                = _rightTeleportInteractor.interactionLayers
                = 0;
        }

        canMove = b;
        //rb.isKinematic = !b;
    }

    public void Teleport(Vector3 point)
    {
        fadeUtil.FadeOutIn(
            .5f,
            () => SetCanMove(false),
            () => { transform.position = point; },
            () => SetCanMove(true));
    }

    public void PathFinding(Vector3 targetPos)
    {
        NavMeshPath path = new NavMeshPath();
        agent.Warp(head.position);
        agent.CalculatePath(targetPos, path);

        line.SetCorners(path.corners);
        if (line.Line.positionCount == 0) return;

        line.gameObject.SetActive(true);
    }

    public float2 HeadXZ()
    {
        float3 headPos = head.position;
        return headPos.xz;
    }

    public void AlignHeadXZ(in Transform target)
    {
        var y = target.position.y;
        var xz = HeadXZ();
        target.position = new(xz.x, y, xz.y);
    }


    #region Hint UI
    public async void ShowWarning()
    {
        hintCanvas.ForceAlign();
        warningUi?.TurnOn();
        await System.Threading.Tasks.Task.Delay(2000);
        warningUi?.TurnOff();
    }
    #endregion
}
