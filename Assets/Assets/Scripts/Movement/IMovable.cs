using System;
using UnityEngine;

public interface IMovable
{
    void Move();
    void StartMove();
    void StopMove();
    void ChangePillar(Transform pillar);
    void StopRotation();
    Vector3 CurrentDirection { get; }
    bool IsRotating { get; }

    Action<bool> OnRotationStateChanged { get; set; }
    Action<Vector3> OnDirectionChanged { get; set; }
}