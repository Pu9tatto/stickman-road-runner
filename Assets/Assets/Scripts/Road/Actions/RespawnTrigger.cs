using UnityEngine;
using System.Collections;

public class RespawnTrigger : RoadTrigger
{
    [SerializeField] private float _respawnDelay = 1f;

    protected override void TriggerAction(GameObject player)
    {
        if(player.TryGetComponent<IMovable>(out IMovable movable))
        {
            movable.StopMove();
        }

        if(player.TryGetComponent<RespawnComponent>(out RespawnComponent respawner))
        {
            StartCoroutine(Respawn(respawner));
        }
    }

    private IEnumerator Respawn(RespawnComponent respawner)
    {
        yield return new WaitForSeconds(_respawnDelay);

        respawner.Respawn();
    }
}


