public class BotMovement : BaseMovement
{
    protected override void Start()
    {
        base.Start();
        StartMove();
    }

    public override void Move()
    {
        base.Move();

        if (_rotationMovement.HasActivePillar)
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

            if(_straightMovement.IsMoving ==  false)
                StartMove();

            _straightMovement.Move();
        }
    }

    public override void ResetToStart()
    {
        base.ResetToStart();

        StartMove();
    }

    public override void SetInputPressed(bool pressed)
    {
    }
}