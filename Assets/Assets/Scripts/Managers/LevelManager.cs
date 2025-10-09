// LevelManager.cs
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField] private List<string> _levelSequence = new List<string> { "level_01", "level_02", "level_03" };
    [SerializeField] private bool _loopLevels = true;
    [SerializeField] private float _levelTransitionDelay = 2f;

    [Header("References")]
    [SerializeField] private LevelBuilder _levelBuilder;
    [SerializeField] private RoadSegmentPool _segmentPool;

    [SerializeField] private GameObject _player;

    private int _currentLevelIndex = -1;
    private string _currentLevelName;
    private bool _isLevelTransitioning = false;

    public static LevelManager Instance { get; private set; }
    public string CurrentLevelName => _currentLevelName;
    public int CurrentLevelIndex => _currentLevelIndex;
    public int TotalLevels => _levelSequence.Count;

    // События для управления геймплеем
    public static event Action OnGamePaused;
    public static event Action OnGameResumed;
    public static event Action OnLevelReset;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadNextLevel();
    }

    private void Update()
    {
        Debug.Log(Time.timeScale);
    }

    public void LoadNextLevel()
    {
        if (_isLevelTransitioning) return;

        _currentLevelIndex++;

        // Проверяем завершение всех уровней
        if (_currentLevelIndex >= _levelSequence.Count)
        {
            if (_loopLevels)
            {
                _currentLevelIndex = 0; // Зацикливаем
            }
            else
            {
                return;
            }
        }

        StartCoroutine(TransitionToLevel(_levelSequence[_currentLevelIndex]));
    }

    private IEnumerator TransitionToLevel(string levelName)
    {
        _isLevelTransitioning = true;
        _currentLevelName = levelName;

        Debug.Log($"Loading level: {levelName}");

        // 1. Пауза через события
        OnGamePaused?.Invoke();

        // 2. Очистка и загрузка
        _levelBuilder?.ClearLevel();
        yield return new WaitForEndOfFrame();

        _levelBuilder?.LoadLevel(levelName);
        yield return new WaitForEndOfFrame();

        // 3. Респавн через события
        OnLevelReset?.Invoke();

        // 4. Задержка
        yield return new WaitForSecondsRealtime(_levelTransitionDelay);

        // 5. Возобновление через события
        OnGameResumed?.Invoke();

        _isLevelTransitioning = false;
    }

    // Вызывается при завершении уровня (например, из FinishTrigger)
    public void CompleteCurrentLevel()
    {
        if (_isLevelTransitioning) return;

        StartCoroutine(DelayedLevelLoad());
    }

    private IEnumerator DelayedLevelLoad()
    {
        yield return new WaitForSeconds(_levelTransitionDelay);
        LoadNextLevel();
    }


    // Методы для UI кнопок
    [ContextMenu("Next Level")]
    public void NextLevel()
    {
        LoadNextLevel();
    }

    [ContextMenu("Restart Level")]
    public void RestartLevel()
    {
        if (_isLevelTransitioning) return;
        StartCoroutine(TransitionToLevel(CurrentLevelName));
    }

    // Для отладки в редакторе
    private void OnGUI()
    {
        if (Debug.isDebugBuild)
        {
            GUILayout.BeginArea(new Rect(10, 10, 200, 100));
            GUILayout.Label($"Current Level: {_currentLevelName}");
            GUILayout.Label($"Level Index: {_currentLevelIndex + 1}/{TotalLevels}");
            GUILayout.Label($"Transitioning: {_isLevelTransitioning}");

            if (GUILayout.Button("Next Level") && !_isLevelTransitioning)
            {
                LoadNextLevel();
            }

            GUILayout.EndArea();
        }
    }
}