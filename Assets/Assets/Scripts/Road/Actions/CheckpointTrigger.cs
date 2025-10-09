using UnityEngine;

public class CheckpointTrigger : RoadTrigger
{
    protected override void TriggerAction(GameObject player)
    {
        if(player.TryGetComponent<RespawnComponent>(out RespawnComponent respawn))
        {
            respawn.SetRespawnPoint(player.transform.position);
            respawn.SetRespawnRotation(player.transform.rotation);

            if (player.TryGetComponent<IMovable>(out IMovable movable))
            {
                respawn.SetDirection(movable.CurrentDirection);
            }
        }
    }
}