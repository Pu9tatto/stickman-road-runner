using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField] private string _levelFileName = "level_01";

    [Header("Dependencies")]
    [SerializeField] private RoadSegmentPool _segmentPool;

    [Header("Road Prefabs")]
    [SerializeField] private StartRoad _startRoadPrefab;
    [SerializeField] private StraightRoad _straightRoadPrefab;
    [SerializeField] private CurveRoad _curvedRoadPrefab_R;
    [SerializeField] private CurveRoad _curvedRoadPrefab_L;
    [SerializeField] private FinishRoad _finishRoadPrefab;


    [Header("Debug")]
    [SerializeField] private List<RoadSegment> _spawnedSegments = new List<RoadSegment>();
    [SerializeField] private Vector3 _startPosition = Vector3.zero;

    private Vector3 _currentPosition;
    private Vector3 _currentDirection;
    private bool _isInitialized = false;

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (_isInitialized) return;

        if (_segmentPool == null)
            _segmentPool = GetComponent<RoadSegmentPool>();

        _segmentPool?.Initialize();
        _isInitialized = true;
    }

    public void LoadLevel(string fileName)
    {
        if (!_isInitialized) Initialize();

        ClearLevel();

        List<PrefabType> levelData = LevelParser.ParseLevelFile(fileName);
        if (levelData.Count == 0)
        {
            Debug.LogError($"Failed to load level: {fileName}");
            return;
        }

        ResetStartParams();

        foreach (PrefabType prefabType in levelData)
        {
            CreateSegmentFromType(prefabType);
        }

        Debug.Log($"Level '{fileName}' loaded successfully with {levelData.Count} segments");

        _segmentPool?.PrintPoolStats();
    }

    private void CreateSegmentFromType(PrefabType prefabType)
    {
        // Получаем сегмент из пула
        RoadSegment segment = _segmentPool.GetSegment(prefabType);
        if (segment == null)
        {
            Debug.LogError($"Failed to get segment from pool for type: {prefabType}");
            return;
        }

        ConfigureSegment(segment, prefabType);

        ModifySegment(segment);

        _spawnedSegments.Add(segment);
    }

    private void ModifySegment(RoadSegment segment)
    {

        // Устанавливаем позицию и поворот
        segment.transform.rotation = Quaternion.Euler(_currentDirection);

        Vector3 startPointLocal = segment.StartPoint.position - segment.transform.position;
        segment.transform.position = _currentPosition - startPointLocal;

        segment.transform.SetParent(transform);

        // Обновляем текущую позицию и направление для следующего сегмента
        _currentPosition = segment.EndPoint.position;
        _currentDirection += segment.Direction;
    }

    private void ConfigureSegment(RoadSegment segment, PrefabType type)
    {
        switch (type)
        {
            case PrefabType.START:
                segment = _startRoadPrefab;
                break;
            case PrefabType.STRAIGHT:
                segment = _straightRoadPrefab;
                break;
            case PrefabType.CURVE_R:
                segment = _curvedRoadPrefab_R;
                break;
            case PrefabType.CURVE_L:
                segment = _curvedRoadPrefab_L;
                break;
            case PrefabType.FINISH:
                segment = _finishRoadPrefab;
                break;
        }
    }

    public void ClearLevel()
    {
        if (_segmentPool != null)
        {
            _segmentPool.ReturnAllSegments(_spawnedSegments);
        }
        else
        {
            // Резервный вариант - уничтожение объектов
            foreach (RoadSegment segment in _spawnedSegments)
            {
                if (segment != null)
                {
                    if (Application.isPlaying)
                        Destroy(segment.gameObject);
                    else
                        DestroyImmediate(segment.gameObject);
                }
            }
        }

        _spawnedSegments.Clear();

        ResetStartParams();
    }

    private void ResetStartParams()
    {
        _currentPosition = _startPosition;
        _currentDirection = Vector3.zero;
    }

    [ContextMenu("Load Level")]
    private void LoadLevelEditor()
    {
        LoadLevel(_levelFileName);
    }

    [ContextMenu("Clear Level")]
    private void ClearLevelEditor()
    {
        ClearLevel();
    }

    public void ReloadWithNewLevel(string newLevelFileName)
    {
        _levelFileName = newLevelFileName;
        LoadLevel(_levelFileName);
    }

}