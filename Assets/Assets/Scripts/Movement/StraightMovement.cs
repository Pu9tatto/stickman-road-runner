using UnityEngine;

public class StraightMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 20f;

    private bool _isMoving = false;
    private Vector3 _currentDirection;

    public Vector3 CurrentDirection => _currentDirection;
    public bool IsMoving => _isMoving;

    public void StartMove(Vector3 direction)
    {
        _isMoving = true;
        //_currentDirection = direction;
    }

    public void StopMove()
    {
        _isMoving = false;
    }

    public void Move()
    {
        if (!_isMoving) return;
        transform.position += _currentDirection * (_moveSpeed * Time.deltaTime);
    }

    public void SetDirection(Vector3 direction)
    {
        _currentDirection = direction;
    }
}