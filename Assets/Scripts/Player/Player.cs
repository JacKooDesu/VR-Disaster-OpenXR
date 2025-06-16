using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using DG.Tweening;

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
    public Transform leftHandler, rightHandler;

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

    private async void Start()
    {
        rb = GetComponent<Rigidbody>();

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

        onTeleportEvent.AddListener(() => agent.Warp(transform.position));

        // 避免過快載入(下下策)
        await System.Threading.Tasks.Task.Delay(800);
        fadeUtil.FadeIn(.5f);
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
        agent.CalculatePath(targetPos, path);

        line.SetCorners(path.corners);
        if (line.Line.positionCount == 0) return;

        line.gameObject.SetActive(true);
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
