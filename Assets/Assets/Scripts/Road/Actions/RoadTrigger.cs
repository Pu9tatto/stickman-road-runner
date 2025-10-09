using UnityEngine;
using UnityEngine.Events;

public abstract class RoadTrigger : MonoBehaviour
{
    [Header("Trigger Settings")]
    [SerializeField] protected UnityEvent OnTriggerActivated;

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IMovable>(out IMovable collider))
        {
            TriggerAction(other.gameObject);
            OnTriggerActivated?.Invoke();
        }
    }

    protected abstract void TriggerAction(GameObject player);
}