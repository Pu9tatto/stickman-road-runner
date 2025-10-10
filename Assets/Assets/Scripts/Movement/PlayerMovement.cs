using UnityEngine;

public class PlayerMovement : MonoBehaviour, IMovable
{
    [Header("Components")]
    [SerializeField] private StraightMovement _straightMovement;
    [SerializeField] private RotationMovement _rotationMovement;
    [SerializeField] private GroundChecker _groundChecker;
    [SerializeField] private FallMovement _fallMovement;

    [Header("Settings")]
    [SerializeField] private Vector3 _startPosition = Vector3.zero;
    [SerializeField] private Vector3 _startDirection = Vector3.forward;

    private bool _inputPressed = false;

    public Vector3 CurrentDirection => _straightMovement.CurrentDirection;
    public bool IsRotating => _rotationMovement.IsRotating;
    public bool IsMoving => _straightMovement.IsMoving && !_fallMovement.IsFalling;
    public bool IsFalling => _fallMovement.IsFalling;

    private void Start()
    {
        ResetToStart();
    }

    private void OnEnable()
    {
        _groundChecker.OnFallStarted += HandleFallStart;
        LevelManager.OnLevelReset += ResetToStart;
    }
    private void OnDisable()
    {
        _groundChecker.OnFallStarted -= HandleFallStart;
        LevelManager.OnLevelReset -= ResetToStart;
    }

    public void Move()
    {
        if (_fallMovement.IsFalling) return;

        if (_inputPressed && (_rotationMovement.IsRotating || _rotationMovement.HasActivePillar))
        {
            if (!_rotationMovement.IsRotating)
            {
                _rotationMovement.StartRotation();
            }
            _rotationMovement.Rotate();
        }
        else
        {
            if (_rotationMovement.IsRotating)
            {
                StopRotation();
            }
            _straightMovement.Move();
        }
    }

    public void SetInputPressed(bool pressed)
    {
        if (_fallMovement.IsFalling)
        {
            _inputPressed = false;
            return;
        }

        bool wasPressed = _inputPressed;
        _inputPressed = pressed;

        if (pressed && !_straightMovement.IsMoving)
        {
            StartMove();
        }

        if (wasPressed && !pressed && _rotationMovement.IsRotating)
        {
            StopRotation();
        }

        if (pressed && !wasPressed && _rotationMovement.HasActivePillar && !_rotationMovement.IsRotating)
        {
            _rotationMovement.StartRotation();
        }
    }

    public void StopRotation()
    {
        _rotationMovement.StopRotation();
        Vector3 snappedDirection = _rotationMovement.GetSnappedDirection();
        _straightMovement.SetDirection(snappedDirection);
    }

    public void StopMove()
    {
        _straightMovement.StopMove();
        _rotationMovement.StopRotation();
    }

    private void StartMove()
    {
        _straightMovement.StartMove(_startDirection);
    }

    private void HandleFallStart(Vector3 fallDirection)
    {
        StopMove();
    }

    public void ResetToStart()
    {
        transform.position = _startPosition;
        transform.rotation = Quaternion.identity;
        _straightMovement.SetDirection(_startDirection);
        _inputPressed = false;
        _fallMovement.ResetFall();
        _rotationMovement.Respawn();
    }
}