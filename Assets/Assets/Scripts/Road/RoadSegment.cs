using UnityEngine;

public class RoadSegment : MonoBehaviour
{
    [Header("Road Segment Settings")]
    [SerializeField] protected Transform _startPoint;
    [SerializeField] protected Transform _endPoint;
    [SerializeField] protected Vector3 _direction = Vector3.zero;
    [SerializeField] private Transform _pillar;

    public Transform StartPoint => _startPoint;
    public Transform EndPoint => _endPoint;
    public Vector3 Direction=> _direction;
    public Transform TryGetPillar()
    {
        if (_pillar)
        {
            return _pillar;
        }

        return null;
    }
}