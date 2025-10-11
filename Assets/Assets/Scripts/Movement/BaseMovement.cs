using UnityEngine;

public abstract class BaseMovement : MonoBehaviour, IMovable
{
    [Header("Movement Components")]
    [SerializeField] protected StraightMovement _straightMovement;
    [SerializeField] protected BaseRotationMovement _rotationMovement;
    [SerializeField] protected GroundChecker _groundChecker;
    [SerializeField] protected FallMovement _fallMovement;

    [Header("Movement Settings")]
    [SerializeField] protected Vector3 _startPosition = Vector3.zero;
    [SerializeField] protected Vector3 _startDirection = Vector3.forward;

    public Vector3 CurrentDirection => _straightMovement.CurrentDirection;
    public bool IsRotating => _rotationMovement.IsRotating;
    public bool IsMoving => _straightMovement.IsMoving && !_fallMovement.IsFalling;
    public bool IsFalling => _fallMovement.IsFalling;

    protected virtual void Start()
    {
        ResetToStart();
    }

    protected virtual void OnEnable()
    {
        _groundChecker.OnFallStarted += HandleFallStart;
        LevelManager.OnLevelReset += ResetToStart;
    }

    protected virtual void OnDisable()
    {
        _groundChecker.OnFallStarted -= HandleFallStart;
        LevelManager.OnLevelReset -= ResetToStart;
    }

    public virtual void Move()
    {
        if (_fallMovement.IsFalling) return;
    }

    public virtual void StopRotation()
    {
        _rotationMovement.StopRotation();
        Vector3 snappedDirection = _rotationMovement.GetSnappedDirection();
        _straightMovement.SetDirection(snappedDirection);
    }

    public virtual void StopMove()
    {
        _straightMovement.StopMove();
        _rotationMovement.StopRotation();
    }

    protected virtual void StartMove()
    {
        _straightMovement.StartMove(_startDirection);
    }

    protected virtual void HandleFallStart(Vector3 fallDirection)
    {
        StopMove();
    }

    public virtual void ResetToStart()
    {
        transform.position = _startPosition;
        transform.rotation = Quaternion.identity;
        _straightMovement.SetDirection(_startDirection);
        _fallMovement.ResetFall();
        _rotationMovement.Respawn();
    }

    // Абстрактные методы для реализации в дочерних классах
    public abstract void SetInputPressed(bool pressed);
}