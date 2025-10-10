using UnityEngine;

public class RespawnComponent : MonoBehaviour
{
    [SerializeField] private FallMovement _faller;
    [SerializeField] private StraightMovement _straight;

    private Vector3 _respawnPoint = Vector3.zero;
    private Quaternion _respawnRotaion = Quaternion.identity;
    public Vector3 Direction { get; private set; }

    private void OnEnable()
    {
        _faller.OnRespawn += Respawn;
    }

    private void OnDisable()
    {
        _faller.OnRespawn -= Respawn;
    }

    public void Respawn()
    {
        transform.position = _respawnPoint;
        transform.rotation = _respawnRotaion;
        _straight.SetDirection(Direction);
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
