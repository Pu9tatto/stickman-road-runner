using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class PoolConfig
{
    public PrefabType Type;
    public RoadSegment Prefab;
    public int PoolSize = 10;
}

public class RoadSegmentPool : MonoBehaviour
{
    [Header("Pool Configuration")]
    [SerializeField] private List<PoolConfig> _poolConfigs = new List<PoolConfig>();

    private Dictionary<PrefabType, Queue<RoadSegment>> _pools = new Dictionary<PrefabType, Queue<RoadSegment>>();
    private Dictionary<PrefabType, RoadSegment> _prefabMap = new Dictionary<PrefabType, RoadSegment>();
    private Dictionary<RoadSegment, PrefabType> _segmentToTypeMap = new Dictionary<RoadSegment, PrefabType>();

    private Transform _poolContainer;

    public void Initialize()
    {
        _poolContainer = new GameObject("PoolContainer").transform;
        _poolContainer.SetParent(transform);

        // Создаем пулы для каждого типа префаба
        foreach (PoolConfig config in _poolConfigs)
        {
            CreatePool(config.Type, config.Prefab, config.PoolSize);
        }

        Debug.Log($"RoadSegmentPool initialized with {_pools.Count} pools");
    }

    private void CreatePool(PrefabType type, RoadSegment prefab, int poolSize)
    {
        if (_pools.ContainsKey(type))
        {
            Debug.LogWarning($"Pool for {type} already exists!");
            return;
        }

        Queue<RoadSegment> pool = new Queue<RoadSegment>();
        _prefabMap[type] = prefab;

        for (int i = 0; i < poolSize; i++)
        {
            RoadSegment segment = Instantiate(prefab, _poolContainer);
            segment.gameObject.SetActive(false);
            pool.Enqueue(segment);
            _segmentToTypeMap[segment] = type;
        }

        _pools[type] = pool;
    }

    public RoadSegment GetSegment(PrefabType type)
    {
        if (!_pools.ContainsKey(type))
        {
            Debug.LogError($"No pool found for type: {type}");
            return null;
        }

        Queue<RoadSegment> pool = _pools[type];
        RoadSegment segment = null;

        if (pool.Count > 0)
        {
            segment = pool.Dequeue();
        }
        else
        {
            // Если пул пуст, создаем новый объект
            Debug.LogWarning($"Pool for {type} is empty, creating new instance");
            segment = Instantiate(_prefabMap[type]);
            _segmentToTypeMap[segment] = type;
        }

        // Активируем сегмент и убираем из контейнера пула
        segment.gameObject.SetActive(true);
        segment.transform.SetParent(null); // Убираем из контейнера пула

        return segment;
    }

    public void ReturnSegment(RoadSegment segment)
    {
        if (segment == null) return;

        // Деактивируем сегмент и возвращаем в контейнер пула
        segment.gameObject.SetActive(false);
        segment.transform.SetParent(_poolContainer);

        if (_segmentToTypeMap.TryGetValue(segment, out PrefabType type))
        {
            if (_pools.ContainsKey(type))
            {
                _pools[type].Enqueue(segment);
            }
            else
            {
                Debug.LogWarning($"No pool found for type: {type}, destroying segment");
                Destroy(segment.gameObject);
            }
        }
        else
        {
            Debug.LogWarning("Segment not from pool, destroying");
            Destroy(segment.gameObject);
        }
    }

    public void ReturnAllSegments(List<RoadSegment> segments)
    {
        // Создаем копию списка чтобы избежать модификации во время итерации
        List<RoadSegment> segmentsToReturn = new List<RoadSegment>(segments);

        foreach (RoadSegment segment in segmentsToReturn)
        {
            ReturnSegment(segment);
        }

        segments.Clear();
    }

    public void ClearAllPools()
    {
        foreach (Transform child in _poolContainer)
        {
            Destroy(child.gameObject);
        }

        _pools.Clear();
        _prefabMap.Clear();
        _segmentToTypeMap.Clear();
    }

    // Статистика пулов для отладки
    public void PrintPoolStats()
    {
        foreach (var pool in _pools)
        {
            Debug.Log($"Pool {pool.Key}: {pool.Value.Count} available");
        }
    }
}