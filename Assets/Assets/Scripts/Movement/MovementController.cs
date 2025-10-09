using UnityEngine;

public class MovementController : MonoBehaviour
{
    private IMovable _movable;

    private void Awake()
    {
        _movable = GetComponent<IMovable>();
    }

    private void Update()
    {
        _movable?.Move();
    }
}