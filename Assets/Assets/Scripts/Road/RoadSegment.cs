using UnityEngine;

public class RoadSegment : MonoBehaviour
{
    [Header("Road Segment Settings")]
    [SerializeField] protected Transform _startPoint;
    [SerializeField] protected Transform _endPoint;
    [SerializeField] protected Vector3 _direction = Vector3.zero;
    public Transform StartPoint => _startPoint;
    public Transform EndPoint => _endPoint;
    public Vector3 Direction=> _direction;
}