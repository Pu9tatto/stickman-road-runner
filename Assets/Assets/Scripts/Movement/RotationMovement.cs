// RotationMovement.cs
using UnityEngine;

public class RotationMovement : MonoBehaviour
{
    [SerializeField] private FallMovement _faller;
    [SerializeField] private float _moveSpeed = 5f;

    private Transform _currentPillar;
    private Vector3 _rotationCenter;
    private float _rotationDirection;
    private float _rotationRadius;
    private float _currentAngle;
    private bool _isRotating = false;

    public bool IsRotating => _isRotating;
    public bool HasActivePillar => _currentPillar != null;

    private void OnEnable()
    {
        _faller.OnRespawn += Respawn;
    }

    private void OnDisable()
    {
        _faller.OnRespawn -= Respawn;
    }

    public void SetPillar(Transform pillar)
    {
        _currentPillar = pillar;
    }

    public void TryClearPillar(Transform pillar)
    {
        if (_currentPillar == pillar)
        {
            _currentPillar = null;
        }
    }

    public void Respawn()
    {
        _currentPillar = null;
        StopRotation();
    }

    public void StartRotation()
    {
        if (_currentPillar == null) return;

        _rotationCenter = _currentPillar.position;
        _rotationRadius = Vector3.Distance(transform.position, _rotationCenter);
        _currentAngle = CalculateStartAngle();
        _rotationDirection = GetRotationDirection();
        _isRotating = true;

    }

    public void StopRotation()
    {
        _isRotating = false;
    }

    public void Rotate()
    {
        if (!_isRotating) return;

        _currentAngle += _moveSpeed * Time.deltaTime / _rotationRadius * _rotationDirection;

        Vector3 newPosition = CalculateNewPosition();
        Vector3 tangentDirection = CalculateTangentDirection(_rotationDirection);

        transform.position = newPosition;
        UpdateDirection(tangentDirection);
    }

    private Vector3 CalculateNewPosition()
    {
        return _rotationCenter + new Vector3(
            Mathf.Sin(_currentAngle) * _rotationRadius,
            transform.position.y,
            Mathf.Cos(_currentAngle) * _rotationRadius
        );
    }

    private Vector3 CalculateTangentDirection(float rotationDirection)
    {
        return new Vector3(
            Mathf.Cos(_currentAngle) * rotationDirection,
            0,
            -Mathf.Sin(_currentAngle) * rotationDirection
        ).normalized;
    }

    private float GetRotationDirection()
    {
        // Если столба нет, используем последнее известное направление
        /*if (_currentPillar == null)
        {
            // Определяем направление по текущему углу
            return Mathf.Sign(Mathf.Cos(_currentAngle) * transform.forward.z - Mathf.Sin(_currentAngle) * transform.forward.x);
        }*/

        Vector3 toPillar = _currentPillar.position - transform.position;
        float crossProduct = Vector3.Cross(transform.forward, toPillar.normalized).y;
        return crossProduct > 0 ? 1f : -1f;
    }

    private float CalculateStartAngle()
    {
        Vector3 toPlayer = transform.position - _rotationCenter;
        return Mathf.Atan2(toPlayer.x, toPlayer.z);
    }

    private void UpdateDirection(Vector3 direction)
    {
        transform.forward = direction;
    }

    public Vector3 GetSnappedDirection()
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
}