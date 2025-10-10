using System.Collections;
using UnityEngine;

public class FinishTrigger : RoadTrigger
{
    [Header("Finish Settings")]
    [SerializeField] private ParticleSystem _confettiEffect;
    [SerializeField] private AudioClip _winSound;
    [SerializeField] private float _completionDelay = 1f;

    private bool _isActivated = false;

    protected override void TriggerAction(PlayerMovement playerMovement)
    {
        if (_isActivated) return;
        _isActivated = true;

        // ������������� ������
        playerMovement.StopMove();

        // ��������� ������������������ ���������� ������
        StartCoroutine(CompleteLevelSequence(playerMovement));
    }

    private IEnumerator CompleteLevelSequence(PlayerMovement playerMovement)
    {
        // �������� ������
        PlayWinAnimation(playerMovement);

        // �������
        PlayWinEffects();

        // ���� ������� ����� ����������� ������
        yield return new WaitForSeconds(_completionDelay);

        // ��������� �������
        LevelManager.Instance?.CompleteCurrentLevel();

        Debug.Log("Level completed! Player reached finish line.");
    }

    private void PlayWinAnimation(PlayerMovement playerMovement)
    {
        StickmanAnimatorController animator = playerMovement.GetComponent<StickmanAnimatorController>();
        animator?.PlayWinAnimation();
    }

    private void PlayWinEffects()
    {
        if (_confettiEffect != null)
        {
            ParticleSystem effect = Instantiate(_confettiEffect, transform.position, Quaternion.identity);
            effect.Play();
        }

        if (_winSound != null)
        {
            AudioSource.PlayClipAtPoint(_winSound, transform.position, 0.7f);
        }
    }

    protected override bool IsValidTarget(Collider collider)
    {
        // ����� ������������ ������ ������� (�� ������)
        return collider.GetComponent<PlayerMovement>() != null &&
               collider.GetComponent<PlayerInputController>() != null;
    }

    private void OnEnable()
    {
        _isActivated = false;
    }

    private void OnDrawGizmos()
    {
        if (!_showDebug) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, GetComponent<Collider>().bounds.size);

        // ������ ������ ������
        Gizmos.color = Color.white;
        Vector3 center = transform.position;
        Gizmos.DrawLine(center + Vector3.left, center + Vector3.right);
        Gizmos.DrawLine(center + Vector3.forward, center + Vector3.back);
    }
}