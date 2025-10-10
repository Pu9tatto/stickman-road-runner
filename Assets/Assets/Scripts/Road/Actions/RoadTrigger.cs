using UnityEngine;
using UnityEngine.Events;

public abstract class RoadTrigger : MonoBehaviour
{
    [Header("Trigger Settings")]
    [SerializeField] protected UnityEvent OnTriggerActivated;

    [Header("Visual Feedback")]
    [SerializeField] protected bool _showDebug = true;

    protected virtual void OnTriggerEnter(Collider other)
    {
        PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            TriggerAction(playerMovement);
            OnTriggerActivated?.Invoke();

            if (_showDebug)
            {
                Debug.Log($"{GetType().Name} activated by player");
            }
        }
    }

    protected virtual bool IsValidTarget(Collider collider)
    {
        // ѕровер€ем наличие PlayerMovement и PlayerInputController (чтобы отличать игрока от ботов)
        return collider.GetComponent<PlayerMovement>() != null &&
               collider.GetComponent<PlayerInputController>() != null;
    }

    protected abstract void TriggerAction(PlayerMovement playerMovement);
}