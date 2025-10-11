using UnityEngine;
using System.Collections;

public class RespawnTrigger : RoadTrigger
{
    [SerializeField] private float _respawnDelay = 1f;


    protected override void TriggerAction(BaseMovement movable)
    {
        movable.StopMove();

        if (movable.gameObject.TryGetComponent<RespawnComponent>(out RespawnComponent respawner))
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


