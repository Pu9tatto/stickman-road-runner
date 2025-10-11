public class PlayerMovement : BaseMovement
{
    private bool _inputPressed = false;
    public override void Move()
    {
        base.Move();

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

    public override void SetInputPressed(bool pressed)
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

    public override void ResetToStart()
    {
        base.ResetToStart();
        _inputPressed = false;
    }
}