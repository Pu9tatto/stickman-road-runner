using UnityEngine;

public class Pillar : MonoBehaviour
{
    [Header("Pillar Settings")]
    [SerializeField] private float _attractionStrength = 1f;

    public float AttractionStrength => _attractionStrength;
}
