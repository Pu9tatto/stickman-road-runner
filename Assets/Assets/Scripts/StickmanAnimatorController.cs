using UnityEngine;

public class StickmanAnimatorController : MonoBehaviour
{
    [Header("Animation Parameters")]
    [SerializeField] private string _runParam = "IsRunning";
    [SerializeField] private string _slideParam = "IsSliding";
    [SerializeField] private string _fallParam = "IsFalling";
    [SerializeField] private string _winParam = "Win";

    private Animator _animator;
    private PlayerMovement _playerMovement;
    private FallMovement _fallMovement;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _playerMovement = GetComponent<PlayerMovement>();
        _fallMovement = GetComponent<FallMovement>();
    }

    private void OnEnable()
    {
        if (_fallMovement != null)
        {
        }
    }

    private void Update()
    {
        UpdateAnimationStates();
    }

    private void UpdateAnimationStates()
    {
        if (_animator == null || _playerMovement == null || _fallMovement == null) return;

        if (_fallMovement.IsFalling)
        {
            // При падении включаем анимацию падения
            _animator.SetBool(_fallParam, true);
            _animator.SetBool(_runParam, false);
            _animator.SetBool(_slideParam, false);
        }
        else
        {
            // Обычные анимации движения
            bool isRunning = _playerMovement.IsMoving && !_playerMovement.IsRotating;
            bool isSliding = _playerMovement.IsRotating;

            _animator.SetBool(_runParam, isRunning);
            _animator.SetBool(_slideParam, isSliding);
            _animator.SetBool(_fallParam, false);
        }
    }

    public void PlayFall()
    {
        if (_animator == null) return;

        _animator.SetBool(_fallParam, true);
        _animator.SetBool(_runParam, false);
        _animator.SetBool(_slideParam, false);
    }

    public void PlayWinAnimation()
    {
        if (_animator == null) return;

        _animator.SetTrigger(_winParam);
        _animator.SetBool(_runParam, false);
        _animator.SetBool(_slideParam, false);
        _animator.SetBool(_fallParam, false);
    }

    public void ResetAnimations()
    {
        if (_animator == null) return;

        _animator.SetBool(_runParam, false);
        _animator.SetBool(_slideParam, false);
        _animator.SetBool(_fallParam, false);
        _animator.ResetTrigger(_winParam);
    }

    // Анимационные события
    public void OnFallStart()
    {
        // Дополнительные эффекты при начале падения
        // ParticleSystemManager.Instance.PlayFallEffect(transform.position);
    }

    public void OnWaterSplash()
    {
        // Эффекты при попадании в воду
        // ParticleSystemManager.Instance.PlaySplashEffect(transform.position);
        // AudioManager.Instance.PlaySplashSound();
    }
}