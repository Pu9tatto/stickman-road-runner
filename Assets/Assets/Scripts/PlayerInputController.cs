using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    private IMovable _movable;

    private void Awake()
    {
        _movable = GetComponent<IMovable>();
    }

    private void Update()
    {
        HandleInput();
        _movable?.Move();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _movable?.SetInputPressed(true);
        }

        if (Input.GetMouseButtonUp(0))
        {
            _movable?.SetInputPressed(false);
        }
    }
}