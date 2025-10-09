using UnityEngine;

public class RespawnComponent : MonoBehaviour
{
    private Vector3 _respawnPoint = Vector3.zero;
    private Quaternion _respawnRotaion = Quaternion.identity;
    public Vector3 Direction { get; private set; }

    public void Respawn()
    {
        transform.position = _respawnPoint;
        transform.rotation = _respawnRotaion;
    }

    public void SetRespawnPoint(Vector3 position)
    {
        _respawnPoint = position;
    }

    public void SetRespawnRotation(Quaternion rotation)
    {
        _respawnRotaion = rotation;
    }

    public void SetDirection(Vector3 direction)
    {
        Direction = direction;
    }
}
