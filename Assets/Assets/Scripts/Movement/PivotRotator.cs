using UnityEngine;

public class PivotRotator : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private bool _smoothRotation = true;

    private IMovable _movable;
    private Vector3 _lastPosition;

    private void Awake()
    {
        _movable = GetComponent<IMovable>();
        _lastPosition = transform.position;
    }

    private void Update()
    {
        RotateTowardsMovement();
    }

    private void RotateTowardsMovement()
    {
        if (_movable == null) return;

        Vector3 targetDirection = GetTargetDirection();

        if (targetDirection == Vector3.zero) return;

        // Вычисляем угол поворота
        float targetAngle = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;

        // Применяем поворот
        if (_smoothRotation)
        {
            SmoothRotate(targetAngle);
        }
        else
        {
            InstantRotate(targetAngle);
        }
    }

    private Vector3 GetTargetDirection()
    {
        // Приоритет: направление из IMovable, затем направление из движения
        if (_movable.CurrentDirection != Vector3.zero)
        {
            return _movable.CurrentDirection;
        }

        // Резервный вариант: направление из фактического движения
        Vector3 actualMovement = transform.position - _lastPosition;
        _lastPosition = transform.position;

        return actualMovement.normalized;
    }

    private void SmoothRotate(float targetAngle)
    {
        Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
    }

    private void InstantRotate(float targetAngle)
    {
        transform.rotation = Quaternion.Euler(0, targetAngle, 0);
    }

    // Метод для принудительного поворота (можно вызывать извне)
    public void ForceRotateTowards(Vector3 direction)
    {
        if (direction == Vector3.zero) return;

        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, targetAngle, 0);
    }

    // Визуализация направления движения
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * 2f);

        if (_movable != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, _movable.CurrentDirection * 1.5f);
        }
    }
}