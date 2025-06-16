using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using CoroutineUtility;

public class InteracableObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("位置重置")]
    public bool positionReset = true;
    public float resetTime;
    protected float timer;
    protected Vector3 originPos;
    protected Quaternion originRotation;
    protected Transform originParent;
    [Header("抓取")]
    public bool canGrab = true;
    public UnityEvent onGrabEvent;
    [Header("放開")]
    public UnityEvent onReleaseEvent;
    protected const int HOVER_LAYER = 22;
    [Header("懸停")]
    [SerializeField] protected bool canHover = false;
    public float hoverTime = 3f;    // 須滿足Hover Time，才執行onHoverEvent
    public UnityEvent onHoverEvent;
    protected Timer hoverTimer;
    protected HoverHandler hoveringHand;

    [SerializeField] protected bool isGrabbing;
    public bool IsGrabbing
    {
        get => isGrabbing;
    }

    public bool Interactable
    {
        set
        {
            interactable = value;
            if (outline != null && !value)
                outline.enabled = false;
        }
        get
        {
            return interactable;
        }
    }
    [SerializeField] protected bool interactable;

    // rigidbody 設定
    protected Rigidbody rig;
    protected bool originIsKinematic;
    protected bool originUseGravity;

    protected Collider col;

    // outline 設定
    protected Outline outline;
    public bool interactableOutline = true;    // 是否開啟outline開關

    [Header("其他")]
    public bool trackVelocity = false;
    protected float currentVelocity;
    public float CurrentVelocity
    {
        get => currentVelocity * velocityMutiply;
    }
    public float velocityMutiply = 100000f;

    protected Vector3 currentPos, lastPos;

    public bool debugVelocity;
    protected Text debugText = null;

    protected virtual void Start()
    {
        SetupOrigin();

        if (GetComponent<Outline>())
        {
            outline = GetComponent<Outline>();
            if (interactableOutline)
                outline.enabled = false;
        }

        if (debugVelocity)
            debugText = GetComponentInChildren<Text>();

        if (canHover)
            hoverTimer = new Timer(hoverTime, () => { }, HoverUpdate, Hovered, false);
    }

    // 定義原位置訊息
    protected void SetupOrigin()
    {
        originPos = transform.position;
        originRotation = transform.rotation;
        originParent = transform.parent;

        if (!GetComponent<Rigidbody>())
            return;

        var rig = GetComponent<Rigidbody>();
        this.rig = rig;
        originIsKinematic = rig.isKinematic;
        originUseGravity = rig.useGravity;

        currentPos = transform.position;
        lastPos = currentPos;

        this.col = GetComponent<Collider>();
    }

    protected virtual void Update()
    {
        // if (!Interactable && isGrabbing)
        //     Released();
        CheckPosReset();

        currentPos = transform.position;
        if (isGrabbing && trackVelocity)
            currentVelocity = (currentPos - lastPos).magnitude;
        else
            currentVelocity = 0f;

        if (debugVelocity && debugText != null)
            debugText.text = $"Velocity = {CurrentVelocity.ToString("F2")}";
        lastPos = currentPos;
    }

    protected void CheckPosReset()
    {
        if (!positionReset)
            return;

        if (isGrabbing)
            return;

        timer += Time.deltaTime;
        if (timer >= resetTime)
            ResetPosition();
    }

    protected virtual void ResetPosition()
    {
        timer = 0f;

        transform.parent = originParent;

        if (rig != null)
        {
            rig.linearVelocity = Vector3.zero;
            rig.isKinematic = originIsKinematic;
            rig.useGravity = originUseGravity;
        }

        transform.SetPositionAndRotation(originPos, originRotation);
    }

    public void Grabbed()
    {
        if (isGrabbing)
            return;

        onGrabEvent.Invoke();
        isGrabbing = true;
    }

    public void Released()
    {
        print($"Has Release {transform.GetInstanceID()}");
        if (!isGrabbing)
            return;

        onReleaseEvent.Invoke();
        isGrabbing = false;
    }

    public void Hovered()
    {
        onHoverEvent.Invoke();

        if (hoveringHand == null)
            return;

        hoveringHand.ResetImage();
        hoveringHand = null;
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (!Interactable)
            return;

        if (!interactableOutline)
            return;

        if (outline != null)
            outline.enabled = true;
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (!Interactable)
            return;

        if (!interactableOutline)
            return;

        if (outline != null)
            outline.enabled = false;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!Interactable)
            return;

        if (!canHover)
            return;

        if (other.gameObject.layer != HOVER_LAYER)
            return;

        if (outline != null)
            outline.enabled = true;

        if (hoverTimer.HasRun)
            return;

        if ((hoveringHand = other.GetComponent<HoverHandler>()) == null)
            return;

        if (onHoverEvent == null)
            return;

        hoverTimer.Start();
    }

    void HoverUpdate(float t)
    {
        if (hoveringHand == null)
            return;

        hoveringHand.UpdateImage(t / hoverTime);
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (!Interactable)
            return;

        if (!canHover)
            return;

        if (other.gameObject.layer != HOVER_LAYER)
            return;

        if (outline != null)
            outline.enabled = true;

        hoverTimer.Stop();

        if (hoveringHand == null)
            return;

        hoveringHand.ResetImage();
        hoveringHand = null;
    }

    public virtual void ResetRig()
    {
        if (rig == null)
            return;

        rig.isKinematic = originIsKinematic;
        rig.useGravity = originUseGravity;
    }

    protected virtual async void ResetCollider()
    {
        col.enabled = false;
        await System.Threading.Tasks.Task.Yield();
        col.enabled = true;
    }

    #region Editor Test
    [ContextMenu("Hover 測試")]
    void HoverTest()
    {
        Hovered();
    }

    [ContextMenu("Grab 測試")]
    void GrabTest()
    {
        Grabbed();
    }

    [ContextMenu("Release 測試")]
    void ReleaseTest()
    {
        Released();
    }
    #endregion
}
