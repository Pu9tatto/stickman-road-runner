using UnityEngine;
using UnityEngine.Events;

public class CheckpointTrigger: MonoBehaviour
{
    [Header("Trigger Settings")]
    [SerializeField] protected UnityEvent OnTriggerActivated;

    [Header("Visual Feedback")]
    [SerializeField] protected bool _showDebug = true;

    private void OnTriggerEnter(Collider player)
    {
        if (player.TryGetComponent<RespawnComponent>(out RespawnComponent respawn))
        {
            respawn.SetRespawnPoint(player.transform.position);
            respawn.SetRespawnRotation(player.transform.rotation);

            if (player.TryGetComponent<IMovable>(out IMovable movable))
            {
                respawn.SetDirection(movable.CurrentDirection);
            }
        }
    }
}