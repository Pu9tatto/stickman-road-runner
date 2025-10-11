using System.Collections;
using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    [Header("Finish Settings")]
    [SerializeField] private ParticleSystem _confettiEffect;
    [SerializeField] private AudioClip _winSound;
    [SerializeField] private float _completionDelay = 1f;

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<BaseMovement>(out BaseMovement movement))
        {
            movement.StopMove();

            // Запускаем последовательность завершения уровня
            StartCoroutine(CompleteLevelSequence(movement));
        }
    }

    private IEnumerator CompleteLevelSequence(BaseMovement baseMovament)
    {

        // Эффекты
        PlayWinEffects();

        // Ждем немного перед завершением уровня
        yield return new WaitForSeconds(_completionDelay);

        // Завершаем уровень
        LevelManager.Instance?.CompleteCurrentLevel();
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, GetComponent<Collider>().bounds.size);

        // Рисуем значок финиша
        Gizmos.color = Color.white;
        Vector3 center = transform.position;
        Gizmos.DrawLine(center + Vector3.left, center + Vector3.right);
        Gizmos.DrawLine(center + Vector3.forward, center + Vector3.back);
    }
}