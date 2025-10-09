using System;
using TMPro.EditorUtilities;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, IMovable
{
    [Header("Movement Settings")]
    [SerializeField] private float _startMoveSpeed = 5f;
    [SerializeField] private LayerMask _roadLayerMask = 1; // Слой дороги
    [SerializeField] private float _groundCheckDistance = 1f;
    [SerializeField] private float _fallSpeed = 10f;
    [SerializeField] private float _checkInterval = 0.1f; // Оптимизация: проверяем не каждый 
    [SerializeField] private float _respownHeight = 0f;
    [SerializeField] private Vector3 _startPosition;

    private Vector3[] _allowedDirections = {
        Vector3.forward,    // 0°
        Vector3.right,      // 90°
        Vector3.back,       // 180°
        Vector3.left        // 270°
    };


    public Vector3 CurrentDirection { get; private set; }
    public bool IsRotating { get; private set; }
    public bool IsGrounded { get; private set; }
    public bool IsFalling { get; private set; }

    public Action<bool> OnRotationStateChanged { get; set; }
    public Action<Vector3> OnDirectionChanged { get; set; }

    private Transform _currentPillar;
    private Vector3 _rotationCenter;
    private float _rotationRadius;
    private float _currentAngle;
    private bool _isMoving;
    private float _lastCheckTime;
    private Vector3 _fallStartPosition;
    private RespawnComponent _respawner;
    private float _moveSpeed = 0f;
    private bool _inputPressed = false;
    private bool _hasPillarForRotation = false;

    private void Start()
    {
        _respawner = GetComponent<RespawnComponent>();
        ResetToStart();
    }

    private void OnEnable()
    {
        LevelManager.OnGamePaused += StopMove;
        LevelManager.OnGameResumed += StartMove;
        LevelManager.OnLevelReset += ResetToStart;
    }

    private void OnDisable()
    {
        LevelManager.OnGamePaused -= StopMove;
        LevelManager.OnGameResumed -= StartMove;
        LevelManager.OnLevelReset -= ResetToStart;
    }

    public void ResetToStart()
    {
        CurrentDirection = Vector3.left;
        IsRotating = false;
        _isMoving = false;
        IsGrounded = true;
        IsFalling = false;
        _moveSpeed = 0f;
        transform.position = _startPosition;
        transform.rotation = Quaternion.identity;
    }

    public void Move()
    {
        if (IsFalling)
        {
            HandleFalling();
            return;
        }

        if (Time.time - _lastCheckTime >= _checkInterval)
        {
            CheckGround();
            _lastCheckTime = Time.time;
        }

        bool shouldRotate = _inputPressed && (_currentPillar != null || _hasPillarForRotation);

        if (shouldRotate)
        {
            if (!IsRotating && _currentPillar != null)
            {
                StartRotation();
            }

            if (IsRotating)
            {
                RotateAroundPillar();
            }
            else
            {
                MoveStraight();
            }
        }
        else
        {
            if (IsRotating)
            {
                StopRotation();
            }
            MoveStraight();
        }

    }

    private void CheckGround()
    {
        RaycastHit hit;
        Vector3 rayStart = transform.position + Vector3.up * 0.1f; // Немного выше чтобы избежать false negative

        bool wasGrounded = IsGrounded;
        IsGrounded = Physics.Raycast(rayStart, Vector3.down, out hit, _groundCheckDistance, _roadLayerMask);

        // Начинаем падение если потеряли землю
        if (wasGrounded && !IsGrounded && !IsFalling)
        {
            StartFalling();
        }
    }

    private void StartFalling()
    {
        IsFalling = true;
        IsGrounded = false;
        _fallStartPosition = transform.position;

        // Уведомляем аниматор
        StickmanAnimatorController animator = GetComponent<StickmanAnimatorController>();
        animator?.PlayFall();

        Debug.Log("Player started falling!");
    }

    private void HandleFalling()
    {
        StopRotation();
        transform.position += Vector3.down * (_fallSpeed * Time.deltaTime);

        if (transform.position.y < _respownHeight)
        {
            Respawn();
        }
    }

    private void Respawn()
    {
        IsFalling = false;
        StopMove();

        _respawner.Respawn();
        CurrentDirection = _respawner.Direction;
        SnapToNearestDirection();
    }

    public void StartMove()
    {
        if (_isMoving) 
            return;

        _moveSpeed = _startMoveSpeed;
        _isMoving = true;
    }

    public void StopMove()
    {
        _moveSpeed = 0;
        _currentPillar = null;
        _isMoving = false;
    }

    private void MoveStraight()
    {
        if (!_isMoving) return;

        transform.position += CurrentDirection * (_moveSpeed * Time.deltaTime);
    }

    private void RotateAroundPillar()
    {
        if (_currentPillar == null && !_hasPillarForRotation) return;
        if (!IsGrounded) return;

        float rotationDirection = GetRotationDirection();
        _currentAngle += _moveSpeed * Time.deltaTime / _rotationRadius * rotationDirection;

        Vector3 newPosition = _rotationCenter + new Vector3(
            Mathf.Sin(_currentAngle) * _rotationRadius,
            transform.position.y,
            Mathf.Cos(_currentAngle) * _rotationRadius
        );

        Vector3 tangentDirection = new Vector3(
            Mathf.Cos(_currentAngle) * rotationDirection,
            0,
            -Mathf.Sin(_currentAngle) * rotationDirection
        );

        transform.position = newPosition;
        SetDirectionInternal(tangentDirection.normalized);
    }

    private float GetRotationDirection()
    {
        if (_currentPillar == null && !_hasPillarForRotation) return 1f;

        // Вектор от стикмена к столбу
        Vector3 toPillar = (_currentPillar != null) ?
            _currentPillar.position - transform.position :
            _rotationCenter - transform.position;

        // Определяем, с какой стороны находится столб относительно направления движения
        float crossProduct = Vector3.Cross(CurrentDirection, toPillar.normalized).y;
        return crossProduct > 0 ? 1f : -1f;
    }

    public void ChangePillar(Transform pillar)
    {
        if(_currentPillar ==  pillar)
        {
            _currentPillar = null;
            return;
        }
        _currentPillar = pillar;
        _rotationCenter = pillar.position;
        //_currentAngle = CalculateStartAngle();
        _hasPillarForRotation = true;

        if (_inputPressed && !IsRotating)
        {
            StartRotation();
        }
    }

    private void StartRotation()
    {
        if (_currentPillar == null) return;

        _rotationRadius = Vector3.Distance(transform.position, _rotationCenter);
        _currentAngle = CalculateStartAngle();

        IsRotating = true;
        _hasPillarForRotation = true;
        SetRotationState(true);
        Debug.Log("Started rotation around pillar");
    }

    public Transform GetCurrentPillar()
    {
        return _currentPillar;
    }

    public void StopRotation()
    {
        IsRotating = false;
        SnapToNearestDirection();

        SetRotationState(false);
    }

    public void SetInputPressed(bool pressed)
    {
        bool wasPressed = _inputPressed;
        _inputPressed = pressed;

        if (pressed && !_isMoving)
        {
            StartMove(); // Начинаем движение при первом нажатии
        }

        // Если отпустили клавишу - останавливаем вращение
        if (wasPressed && !pressed && IsRotating)
        {
            StopRotation();
        }

        // Если нажали клавишу и есть столб - начинаем вращение
        if (pressed && !wasPressed && _currentPillar != null && !IsRotating)
        {
            StartRotation();
        }
    }

    private float CalculateStartAngle()
    {
        Vector3 toPlayer = transform.position - _rotationCenter;
        return Mathf.Atan2(toPlayer.x, toPlayer.z);
    }

    private void SnapToNearestDirection()
    {
        // Находим ближайшее разрешенное направление
        Vector3 nearestDirection = _allowedDirections[0];
        float maxDot = -1f;

        foreach (Vector3 direction in _allowedDirections)
        {
            float dot = Vector3.Dot(CurrentDirection, direction);
            if (dot > maxDot)
            {
                maxDot = dot;
                nearestDirection = direction;
            }
        }

        SetDirectionInternal(nearestDirection);
    }
    private void SetDirectionInternal(Vector3 newDirection)
    {
        if (CurrentDirection != newDirection)
        {
            CurrentDirection = newDirection;
            OnDirectionChanged?.Invoke(newDirection);
        }
    }

    private void SetRotationState(bool isRotating)
    {
        OnRotationStateChanged?.Invoke(isRotating);
    }

    // Визуализация для дебага
    private void OnDrawGizmos()
    {
        Gizmos.color = IsGrounded ? Color.black : Color.red;
        Vector3 rayStart = transform.position + Vector3.up * 0.1f;
        Gizmos.DrawRay(rayStart, Vector3.down * _groundCheckDistance);

        if (IsFalling)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }

        if (IsRotating && _currentPillar != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_rotationCenter, _rotationRadius);
            Gizmos.DrawLine(transform.position, _rotationCenter);
        }

        // Рисуем направление движения
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, CurrentDirection * 2f);
    }

}