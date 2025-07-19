using UnityEngine;
using UnityEngine.Events;

public class KnockableObject : MonoBehaviour
{
    [Header("敲擊")]
    [field: SerializeField]
    public bool CanKnock { get; set; } = false;
    [SerializeField] Vector3 _knockDirection = Vector3.up;

    Vector3 _knockDirectionConverted;
    [SerializeField] float _knockThreshold = 0.1f;
    public UnityEvent OnKnocked = new();

    void Awake()
    {
        _knockDirectionConverted = transform.TransformDirection(_knockDirection);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!CanKnock)
            return;

        if (other.TryGetComponent<KnockSource>(out var source))
            CheckKnock(source);
    }

    void CheckKnock(KnockSource source)
    {
        var currentVelocity = source.CurrentVelocity;
        if (_knockDirection != Vector3.zero)
            currentVelocity = Vector3.Project(currentVelocity, _knockDirectionConverted);

        if (currentVelocity.magnitude >= _knockThreshold)
            OnKnocked.Invoke();
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (_knockDirection == Vector3.zero)
            return;

        Gizmos.DrawLine(transform.position, transform.position + transform.TransformDirection(_knockDirection).normalized);
    }
#endif
}
