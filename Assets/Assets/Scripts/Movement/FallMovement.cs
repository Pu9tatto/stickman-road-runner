using UnityEngine;
using System;

public class FallMovement : MonoBehaviour
{
    [Header("Fall Settings")]
    [SerializeField] private float _fallSpeed = 8f;
    [SerializeField] private float _fallRotationSpeed = 90f;
    [SerializeField] private float _respownHeight = -10f;
     
    private RotationMovement _rotator;
    private GroundChecker _groundChecker;
    private bool _isFalling = false;
    private Vector3 _fallDirection;
    private Vector3 _fallStartPosition;

    public bool IsFalling => _isFalling;
    public Action OnRespawn;

    private void Awake()
    {
        _groundChecker = GetComponent<GroundChecker>();
    }

    private void OnEnable()
    {
        if (_groundChecker != null)
        {
            _groundChecker.OnFallStarted += StartFalling;
        }
    }

    private void OnDisable()
    {
        if (_groundChecker != null)
        {
            _groundChecker.OnFallStarted -= StartFalling;
        }
    }

    private void Update()
    {
        if (_isFalling)
        {
            HandleFalling();
        }
    }

    private void StartFalling(Vector3 direction)
    {
        if (_isFalling) return;

        _isFalling = true;
        _fallDirection = direction;
        _fallStartPosition = transform.position;
    }

    private void HandleFalling()
    {
        // Двигаем по диагонали вниз
        transform.position += _fallDirection * (_fallSpeed * Time.deltaTime);

        // Добавляем вращение при падении
        transform.Rotate(Vector3.forward, _fallRotationSpeed * Time.deltaTime);

        // Проверяем достижение нижней границы
        if (transform.position.y < _respownHeight)
        {
            Respawn();
        }
    }

    private void Respawn()
    {
        _isFalling = false;
        OnRespawn.Invoke();
    }

    public void StopFalling()
    {
        _isFalling = false;
    }

    public void ResetFall()
    {
        _isFalling = false;
        transform.rotation = Quaternion.identity;
    }
}