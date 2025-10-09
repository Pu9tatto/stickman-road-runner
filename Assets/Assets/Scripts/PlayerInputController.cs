using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    private PlayerMovement _playerMovement;

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        // Нажатие - начинаем вращение если есть столб
        if (Input.GetMouseButtonDown(0))
        {
            _playerMovement.SetInputPressed(true);
        }

        // Отпускание - прекращаем вращение
        if (Input.GetMouseButtonUp(0))
        {
            _playerMovement.SetInputPressed(false);
        }
    }
}