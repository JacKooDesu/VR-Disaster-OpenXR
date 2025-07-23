using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using CoroutineUtility;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Attachment;

public class InteracableObject : MonoBehaviour
{
    [SerializeField]
    XRGrabInteractable _interactableProxy;

    [Header("位置重置")]
    public bool positionReset = true;
    public float resetTime;
    protected float timer;
    protected Vector3 originPos;
    protected Quaternion originRotation;
    protected Transform originParent;
    [Header("抓取")]
    public bool canGrab = true;
    public UnityEvent OnInteracted;
    [FormerlySerializedAs("onGrabEvent")]
    public UnityEvent OnGrabbed;
    public UnityEvent OnFishing;
    [Header("放開")]
    public UnityEvent onReleaseEvent;
    protected const int HOVER_LAYER = 22;
    [Header("懸停")]
    [SerializeField] protected bool canHover = false;
    public float hoverTime = 3f;    // 須滿足Hover Time，才執行onHoverEvent
    public UnityEvent onHoverEvent;
    protected Timer hoverTimer;
    protected HoverHandler hoveringHand;

    public bool IsFishing { get; private set; }

    public enum EGrabMode
    {
        None,
        Fishing,
        Pulling,
        Grabbed
    }

    [field: SerializeField]
    public EGrabMode GrabMode { get; protected set; } = EGrabMode.None;
    public bool IsGrabbing
    {
        get => GrabMode != EGrabMode.None;
    }

    public bool Interactable
    {
        set
        {
            interactable = value;
            if (_outline != null && !value)
                _outline.enabled = false;
            if (_interactableProxy != null)
                _interactableProxy.enabled = value;
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
    protected Outline _outline;
    public Outline Outline => _outline;
    public bool interactableOutline = true;    // 是否開啟outline開關

    protected Vector3 currentPos, lastPos;

    protected InteractableFarAttachMode _originAttachMode;

    public bool debugVelocity;
    protected Text debugText = null;

    void Awake()
    {
        // Read editor setting of interactable
        Interactable = interactable;
    }

    protected virtual void Start()
    {
        SetupOrigin();

        BindInteractable();

        if (TryGetComponent<Outline>(out var outline))
        {
            _outline = outline;
            if (interactableOutline)
                _outline.enabled = false;

            _interactableProxy.hoverEntered.AddListener(arg => OnBeginSelecting());
            _interactableProxy.hoverExited.AddListener(arg => OnEndSelecting());
        }

        if (debugVelocity)
            debugText = GetComponentInChildren<Text>();

        // if (canHover)
        //     hoverTimer = new Timer(hoverTime, () => { }, HoverUpdate, Hovered, false);
    }

    // 定義原位置訊息
    protected void SetupOrigin()
    {
        originPos = transform.localPosition;
        originRotation = transform.localRotation;
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

    protected virtual void BindInteractable()
    {
        if (_interactableProxy == null ||
            !TryGetComponent<XRGrabInteractable>(out var interactable))
            return;

        _interactableProxy = interactable;
    }

    protected virtual void Update()
    {
        // if (!Interactable && isGrabbing)
        //     Released();
        CheckPosReset();

        currentPos = transform.position;

        lastPos = currentPos;
    }

    protected void CheckPosReset()
    {
        if (!positionReset)
            return;

        if (IsGrabbing)
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

        transform.localPosition = originPos;
        transform.localRotation = originRotation;
    }

    public void UpdateGrabState(EGrabMode mode)
    {
        if (GrabMode is EGrabMode.None &&
            mode is EGrabMode.Fishing or EGrabMode.Grabbed)
            OnInteracted?.Invoke();

        var trigger = mode switch
        {
            EGrabMode.Fishing => OnFishing,
            EGrabMode.Grabbed => OnGrabbed,
            _ => null
        };
        trigger?.Invoke();
        GrabMode = mode;
    }

    public void Released()
    {
        if (!IsGrabbing)
            return;

        onReleaseEvent.Invoke();
        GrabMode = EGrabMode.None;
    }

    public void Hovered()
    {
        onHoverEvent.Invoke();

        if (hoveringHand == null)
            return;

        hoveringHand.ResetImage();
        hoveringHand = null;
    }

    public virtual void OnBeginSelecting()
    {
        if (!Interactable)
            return;

        if (!interactableOutline)
            return;

        if (_outline != null)
            _outline.enabled = true;
    }

    public virtual void OnEndSelecting()
    {
        if (!Interactable)
            return;

        if (!interactableOutline)
            return;

        if (_outline != null)
            _outline.enabled = false;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!Interactable)
            return;

        if (!canHover)
            return;

        if (other.gameObject.layer != HOVER_LAYER)
            return;

        if (_outline != null)
            _outline.enabled = true;

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

        if (_outline != null)
            _outline.enabled = true;

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
        UpdateGrabState(EGrabMode.Grabbed);
    }

    [ContextMenu("Release 測試")]
    void ReleaseTest()
    {
        Released();
    }
    #endregion
}
