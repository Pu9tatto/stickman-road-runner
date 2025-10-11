// BotRotationMovement.cs
using UnityEngine;

public class BotRotationMovement : BaseRotationMovement
{
    private bool _hasRotationStarted = false;

    public bool HasRotationStarted => _hasRotationStarted;

    public override void SetPillar(Transform pillar)
    {
        if (_currentPillar == pillar)
        {
            _currentPillar = null;
            return;
        }

        _currentPillar = pillar;

        if (pillar != null)
        {
            // Бот сразу вычисляет идеальные параметры вращения
            _rotationCenter = pillar.position;
            _rotationRadius = Vector3.Distance(transform.position, _rotationCenter);
            _currentAngle = CalculateStartAngle();
            _rotationDirection = GetRotationDirection();
        }
    }

    public override void TryClearPillar(Transform pillar)
    {
        if (_currentPillar == pillar)
        {
            _currentPillar = null;
        }
    }

    public override void StartRotation()
    {
        if (_currentPillar == null) return;

        base.StartRotation();

        _hasRotationStarted = true;
    }

    public override void StopRotation()
    {
        base.StopRotation();

        _currentPillar = null;
        _hasRotationStarted = false;
    }

    public override void Rotate()
    {
        if (!_isRotating) return;

        base.Rotate();
    }

    public override void Respawn()
    {
        base.Respawn();
        _hasRotationStarted = false;
    }
}