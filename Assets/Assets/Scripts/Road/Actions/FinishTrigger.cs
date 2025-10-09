using UnityEngine;

public class FinishTrigger : RoadTrigger
{
    [Header("Finish Settings")]
    [SerializeField] private ParticleSystem _confettiEffect;
    [SerializeField] private AudioClip _winSound;

    protected override void TriggerAction(GameObject player)
    {
        IMovable movable = player.GetComponent<IMovable>();
        if (movable != null)
        {
            movable.StopMove();
        }

        // ¬оспроизводим анимацию победы
        StickmanAnimatorController animator = player.GetComponent<StickmanAnimatorController>();
        animator?.PlayWinAnimation();

        // Ёффекты
        PlayWinEffects();

        LevelManager.Instance?.CompleteCurrentLevel();

        Debug.Log("Finish reached! Level completed!");
    }

    private void PlayWinEffects()
    {
        if (_confettiEffect != null)
            Instantiate(_confettiEffect, transform.position, Quaternion.identity);

        if (_winSound != null)
            AudioSource.PlayClipAtPoint(_winSound, transform.position);
    }
}