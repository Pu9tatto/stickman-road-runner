using UnityEngine;

public class StickmanAnimatorController : MonoBehaviour
{
    [Header("Animation Parameters")]
    [SerializeField] private string _runParam = "IsRunning";
    [SerializeField] private string _slideParam = "StartSlide";
    [SerializeField] private string _fallParam = "IsFalling";
    [SerializeField] private string _winParams = "Win";

    private Animator _animator;
    private IMovable _movable;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _movable = GetComponent<IMovable>();
    }

    private void OnEnable()
    {
        _movable.OnRotationStateChanged += ChangeAnimation;
    }

    private void OnDisable()
    {
        _movable.OnRotationStateChanged -= ChangeAnimation;
    }

    private void ChangeAnimation(bool isRotation)
    {
        if (isRotation)
        {
            PlaySlide();
        }
        else
        {
            SetRunning(true);
        }
    }

    public void PlayFall()
    {
        _animator.SetTrigger(_fallParam);
    }

    public void SetRunning(bool running)
    {
        _animator.SetBool(_runParam, running);
    }

    public void PlaySlide()
    {
        _animator.SetTrigger(_slideParam);
        _animator.SetBool(_runParam, false);
    }

    public void PlayWinAnimation()
    {
        _animator.SetTrigger(_winParams);
    }
}