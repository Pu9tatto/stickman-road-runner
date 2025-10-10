using System;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [Header("Ground Check Settings")]
    [SerializeField] private LayerMask _roadLayerMask = 1;
    [SerializeField] private float _groundCheckDistance = 1f;
    [SerializeField] private float _checkInterval = 0.1f;

    [Header("Fall Settings")]
    [SerializeField] private float _fallAngle = 45f; // ���� ������� � ��������

    private float _lastCheckTime;
    private bool _isGrounded = true;
    private Vector3 _fallDirection;

    public bool IsGrounded => _isGrounded;
    public Vector3 FallDirection => _fallDirection;
    public Action<Vector3> OnFallStarted; // ������ �������� �����������

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

        _isGrounded = Physics.Raycast(rayStart, Vector3.down, _groundCheckDistance, _roadLayerMask);

        if (wasGrounded && !_isGrounded)
        {
            CalculateFallDirection();
            OnFallStarted?.Invoke(_fallDirection);
        }
    }

    private void CalculateFallDirection()
    {
        // ��������� ����������� ������� �� ������ �������� ��������
        Vector3 currentDirection = transform.forward;

        // ������� ������������ ����������� ����
        _fallDirection = (currentDirection + Vector3.down).normalized;

        // ����� ������������ ���� �������
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

        // ������������ ����������� �������
        if (!_isGrounded)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, _fallDirection * 3f);
        }
    }
}