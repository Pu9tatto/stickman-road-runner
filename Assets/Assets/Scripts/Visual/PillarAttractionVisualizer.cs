using UnityEngine;

public class PillarAttractionVisualizer : MonoBehaviour
{
    [Header("Visualization Settings")]
    [SerializeField] private LineRenderer _attractionLine;
    [SerializeField] private ParticleSystem _attractionParticles;
    [SerializeField] private Gradient _attractionGradient;
    [SerializeField] private float _lineWidth = 0.1f;
    [SerializeField] private float _maxLineLength = 10f;

    [Header("Animation Settings")]
    [SerializeField] private float _pulseSpeed = 2f;
    [SerializeField] private float _maxPulseIntensity = 1.5f;

    private BaseRotationMovement _rotationMovement;
    private Transform _currentPillar;
    private bool _isVisualizing = false;
    private Material _lineMaterial;

    private void Awake()
    {
        _rotationMovement = GetComponent<BaseRotationMovement>();
        InitializeLineRenderer();
    }

    private void InitializeLineRenderer()
    {
        if (_attractionLine == null)
        {
            _attractionLine = gameObject.AddComponent<LineRenderer>();
        }

        _attractionLine.positionCount = 2;
        _attractionLine.startWidth = _lineWidth;
        _attractionLine.endWidth = _lineWidth;
        _attractionLine.colorGradient = _attractionGradient;
        _attractionLine.material = new Material(Shader.Find("Sprites/Default"));
        _attractionLine.enabled = false;

        _lineMaterial = _attractionLine.material;
    }

    private void Update()
    {
        UpdateVisualization();
    }

    private void UpdateVisualization()
    {
        bool shouldVisualize = _rotationMovement.IsRotating;

        if (shouldVisualize && !_isVisualizing)
        {
            StartVisualization();
        }
        else if (!shouldVisualize && _isVisualizing)
        {
            StopVisualization();
        }

        if (_isVisualizing)
        {
            UpdateAttractionLine();
        }
    }

    private void StartVisualization()
    {
        _isVisualizing = true;
        _attractionLine.enabled = true;

        if (_attractionParticles != null)
        {
            _attractionParticles.Play();
        }
    }

    private void StopVisualization()
    {
        _isVisualizing = false;
        _attractionLine.enabled = false;

        if (_attractionParticles != null)
        {
            _attractionParticles.Stop();
        }
    }

    private void UpdateAttractionLine()
    {
        if (!_rotationMovement.IsRotating) return;

        Vector3 pillarPosition = GetPillarPosition();
        Vector3 playerPosition = transform.position;

        // Устанавливаем позиции линии
        _attractionLine.SetPosition(0, playerPosition);
        _attractionLine.SetPosition(1, pillarPosition);
    }

    private void ChangeLineAlpha(Vector3 pillarPosition, Vector3 playerPosition)
    {
        float distance = Vector3.Distance(playerPosition, pillarPosition);
        float alpha = Mathf.Clamp01(1f - (distance / _maxLineLength));
        UpdateLineAlpha(alpha);
    }

    private void UpdateLineAlpha(float alpha)
    {
        Color startColor = _attractionGradient.Evaluate(0f);
        Color endColor = _attractionGradient.Evaluate(1f);

        startColor.a = alpha;
        endColor.a = alpha * 0.5f; // Конец линии более прозрачный

        _attractionLine.startColor = startColor;
        _attractionLine.endColor = endColor;
    }

    private Vector3 GetPillarPosition()
    {
        if (_rotationMovement.HasActivePillar)
        {
            Vector3 pillarPosition = _rotationMovement.GetPillar().position;
            pillarPosition.y = transform.position.y;
            return pillarPosition;
        }

        return transform.position;
    }

    // Методы для внешнего управления
    public void ForceStartVisualization(Transform pillar)
    {
        _currentPillar = pillar;
        StartVisualization();
    }

    public void ForceStopVisualization()
    {
        StopVisualization();
    }
}