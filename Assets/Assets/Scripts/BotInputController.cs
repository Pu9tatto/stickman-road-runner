using UnityEngine;

public class BotInputController : MonoBehaviour
{
    private BotMovement _botMovement;

    private void Awake()
    {
        _botMovement = GetComponent<BotMovement>();
    }

    private void Update()
    {
        _botMovement?.Move();
    }
}