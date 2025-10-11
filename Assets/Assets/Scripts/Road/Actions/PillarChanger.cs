using UnityEngine;

public class PillarChanger : RoadTrigger
{
    [SerializeField] private Transform _pillar;
    [SerializeField] private PillarChanger _otherPillar;

    private bool _isExitPillar = false;

    protected override void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<PlayerRotationMovement>(out PlayerRotationMovement _rotator))
        {
            if (_isExitPillar)
            {
                _rotator.TryClearPillar(_pillar);
                _isExitPillar = false;
            }
            else
            {
                _rotator.SetPillar(_pillar);
                _otherPillar.SetExit(true);
            }
        }
    }

    public void SetExit (bool isExit) => _isExitPillar = isExit;

    protected override void TriggerAction(BaseMovement baseMovament)
    {
    }
}
