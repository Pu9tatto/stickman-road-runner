// IMovable.cs
using UnityEngine;

public interface IMovable
{
    void Move();
    void SetInputPressed(bool pressed);
    void StopRotation();
    void StopMove();

    Vector3 CurrentDirection { get; }
    bool IsRotating { get; }
    bool IsMoving { get; }
    bool IsFalling { get; }
}