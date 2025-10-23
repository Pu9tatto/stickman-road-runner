using UnityEngine;

public abstract class BaseRotationMovement : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private FallMovement _faller;
    [SerializeField] private GroundChecker _groundChecker;
    [SerializeField] protected float _moveSpeed = 5f;

    protected Transform _currentPillar;
    protected Vector3 _rotationCenter;
    protected float _rotationDirection;
    protected float _rotationRadius;
    protected float _currentAngle;
    protected bool _isRotating = false;

    public bool IsRotating => _isRotating;
    public bool HasActivePillar => _currentPillar != null;

    private void OnEnable()
    {
        _faller.OnRespawn += Respawn;
        _groundChecker.OnRoadSegmentChanged += SetPillar;
    }

    private void OnDisable()
    {
        _faller.OnRespawn -= Respawn;
        _groundChecker.OnRoadSegmentChanged -= SetPillar;
    }

    public abstract void SetPillar(Transform pillar);
    public abstract void TryClearPillar(Transform pillar);

    public virtual void StartRotation()
    {
        if (_currentPillar == null) return;

        _rotationCenter = _currentPillar.position;
        _rotationRadius = Vector3.Distance(transform.position, _rotationCenter);
        _currentAngle = CalculateStartAngle();
        _rotationDirection = GetRotationDirection();
        _isRotating = true;
    }

    public virtual void StopRotation()
    {
        _isRotating = false;
    }

    public virtual void Rotate()
    {
        if (!_isRotating) return;

        _currentAngle += _moveSpeed * Time.deltaTime / _rotationRadius * _rotationDirection;

        Vector3 newPosition = CalculateNewPosition();
        Vector3 tangentDirection = CalculateTangentDirection(_rotationDirection);

        transform.position = newPosition;
        UpdateDirection(tangentDirection);
    }

    public virtual void Respawn()
    {
        _currentPillar = null;
        StopRotation();
    }

    public virtual Vector3 GetSnappedDirection()
    {
        Vector3[] allowedDirections = { Vector3.forward, Vector3.right, Vector3.back, Vector3.left };
        Vector3 nearestDirection = allowedDirections[0];
        float maxDot = -1f;

        foreach (Vector3 direction in allowedDirections)
        {
            float dot = Vector3.Dot(transform.forward, direction);
            if (dot > maxDot)
            {
                maxDot = dot;
                nearestDirection = direction;
            }
        }
        return nearestDirection;
    }

    // Общие защищенные методы
    protected Vector3 CalculateNewPosition()
    {
        return _rotationCenter + new Vector3(
            Mathf.Sin(_currentAngle) * _rotationRadius,
            transform.position.y,
            Mathf.Cos(_currentAngle) * _rotationRadius
        );
    }

    protected Vector3 CalculateTangentDirection(float rotationDirection)
    {
        return new Vector3(
            Mathf.Cos(_currentAngle) * rotationDirection,
            0,
            -Mathf.Sin(_currentAngle) * rotationDirection
        ).normalized;
    }

    protected float GetRotationDirection()
    {
        Vector3 toPillar = _currentPillar.position - transform.position;
        float crossProduct = Vector3.Cross(transform.forward, toPillar.normalized).y;
        return crossProduct > 0 ? 1f : -1f;
    }

    protected float CalculateStartAngle()
    {
        Vector3 toPlayer = transform.position - _rotationCenter;
        return Mathf.Atan2(toPlayer.x, toPlayer.z);
    }

    protected void UpdateDirection(Vector3 direction)
    {
        transform.forward = direction;
    }

    public Transform GetPillar()
    {
        return _currentPillar;
    }
}