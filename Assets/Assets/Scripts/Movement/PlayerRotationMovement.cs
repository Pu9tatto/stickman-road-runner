using UnityEngine;

public class PlayerRotationMovement : BaseRotationMovement
{
    public override void SetPillar(Transform pillar)
    {
        _currentPillar = pillar;
    }

    public override void TryClearPillar(Transform pillar)
    {
        if (_currentPillar == pillar)
        {
            _currentPillar = null;
        }
    }
}