using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class KnockSource : MonoBehaviour
{
    public Vector3 CurrentVelocity { get; private set; } = Vector3.zero;

    [SerializeField] int _trackingFrames = 10;

    [SerializeField] InteracableObject _interactProxy;
    Collider _collider;

    Vector3[] _trackingPositions;
    bool _isTracking = false;
    int _currentFrame = 0;

    void Awake()
    {
        _interactProxy = GetComponent<InteracableObject>();
        _collider = GetComponent<Collider>();

        _collider.isTrigger = true;

        _trackingPositions = Enumerable
            .Repeat(Vector3.zero, _trackingFrames)
            .ToArray();

        _interactProxy.OnGrabbed.AddListener(() =>
        {
            _isTracking = true;
        });

        _interactProxy.onReleaseEvent.AddListener(
            () => Reset());
    }

    void Update()
    {
        if (!_isTracking)
            return;

        Track();
    }

    void Track()
    {
        var currentPosition = transform.position;

        if (_currentFrame > _trackingFrames)
            CurrentVelocity = currentPosition - _trackingPositions.Aggregate(Vector3.zero, (acc, pos) => acc + pos) / _trackingFrames;

        _trackingPositions[_currentFrame % _trackingFrames] = currentPosition;

        _currentFrame++;
    }

    void Reset()
    {
        _isTracking = false;
        CurrentVelocity = Vector3.zero;
        _currentFrame = 0;
    }
}
