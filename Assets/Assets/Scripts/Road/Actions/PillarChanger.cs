
using UnityEngine;

public class PillarChanger : RoadTrigger
{
    [SerializeField] private Transform _pillar;

    protected override void TriggerAction(GameObject player)
    {
        IMovable movable = player.GetComponent<IMovable>();
        if (movable != null)
        {
            movable.ChangePillar(_pillar);
        }
    }
}
