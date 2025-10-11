using UnityEngine;

public class BotCurveBeginner : MonoBehaviour
{
    [SerializeField] private Transform _pillar;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<BotRotationMovement>(out BotRotationMovement _botRotator))
        {
            _botRotator.SetPillar(_pillar);
        }
    }
}