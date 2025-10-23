using System;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [Header("Ground Check Settings")]
    [SerializeField] private LayerMask _roadLayerMask = 1;
    [SerializeField] private float _groundCheckDistance = 1f;
    [SerializeField] private float _checkInterval = 0.1f;

    [Header("Fall Settings")]
    [SerializeField] private float _fallAngle = 45f; // Угол падения в градусах

    private float _lastCheckTime;
    private bool _isGrounded = true;
    private Vector3 _fallDirection;
    private Transform _lastHitObject;
    private RoadSegment _groundedRoad;

    public bool IsGrounded => _isGrounded;
    public Vector3 FallDirection => _fallDirection;
    public RoadSegment Road => _groundedRoad;

    public Action<Vector3> OnFallStarted; // Теперь передаем направление
    public Action<Transform> OnRoadSegmentChanged;

    private void Update()
    {
        if (Time.time - _lastCheckTime >= _checkInterval)
        {
            CheckGround();
            _lastCheckTime = Time.time;
        }
    }

    private void CheckGround()
    {
        Vector3 rayStart = transform.position + Vector3.up * 0.1f;
        bool wasGrounded = _isGrounded;
        RaycastHit hit;

        _isGrounded = Physics.Raycast(rayStart, Vector3.down, out hit, _groundCheckDistance, _roadLayerMask);

        if (wasGrounded && !_isGrounded)
        {
            CalculateFallDirection();
            OnFallStarted?.Invoke(_fallDirection);
        }

        if (_isGrounded && hit.collider != null)
        {
            if (_lastHitObject != hit.collider.transform)
            {
                _lastHitObject = hit.collider.transform;

                _groundedRoad = hit.collider.transform.parent.GetComponent<RoadSegment>();

                Transform pillarTransform = _groundedRoad.TryGetPillar();

                    OnRoadSegmentChanged?.Invoke(pillarTransform);
            }
        }
        else if (!_isGrounded)
        {
            _lastHitObject = null;
        }
    }

    private void CalculateFallDirection()
    {
        // Вычисляем направление падения на основе текущего движения
        Vector3 currentDirection = transform.forward;

        // Создаем диагональное направление вниз
        _fallDirection = (currentDirection + Vector3.down).normalized;

        // Можно регулировать угол падения
        float angleInRadians = _fallAngle * Mathf.Deg2Rad;
        _fallDirection = new Vector3(
            currentDirection.x * Mathf.Sin(angleInRadians),
            -Mathf.Cos(angleInRadians),
            currentDirection.z * Mathf.Sin(angleInRadians)
        ).normalized;

        Debug.Log($"Fall direction calculated: {_fallDirection}");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = _isGrounded ? Color.green : Color.red;
        Vector3 rayStart = transform.position + Vector3.up * 0.1f;
        Gizmos.DrawRay(rayStart, Vector3.down * _groundCheckDistance);

        // Визуализация направления падения
        if (!_isGrounded)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, _fallDirection * 3f);
        }
    }
}