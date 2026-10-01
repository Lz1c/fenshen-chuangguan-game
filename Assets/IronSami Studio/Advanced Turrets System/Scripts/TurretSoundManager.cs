using UnityEngine;
namespace IronSamiStudio.AdvancedTurretAI {
public class TurretSoundManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSource; // AudioSource for playback

    [Header("Single Sound Settings")]
    [SerializeField] private AudioClip singleShotSound; // Optional single sound
    [SerializeField, Range(0f, 1f)] private float singleShotVolume = 1f;

    [Header("Random Sound Settings")]
    [SerializeField] private AudioClip[] randomShotSounds; // Array for random sounds
    [SerializeField, Range(0f, 1f)] private float randomShotVolume = 1f;
    [SerializeField] private bool useRandomSounds = false; // Toggle for random vs single sound

    [Header("3D Audio Settings")]
    [Range(0f, 1f)] public float spatialBlend = 1f; // 0 = 2D, 1 = 3D
    [Range(0f, 500f)] public float maxDistance = 50f; // Max distance for sound falloff
    public AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic; // Sound falloff type

    void Awake()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                Debug.Log("Added AudioSource to TurretSoundManager", gameObject);
            }
        }

        // Configure AudioSource for 3D sound
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = spatialBlend; // Set to 3D by default (1f)
        audioSource.maxDistance = maxDistance;   // Distance where sound becomes inaudible
        audioSource.rolloffMode = rolloffMode;   // How volume decreases with distance
    }

    /// <summary>
    /// Plays either the single shot sound or a random sound based on useRandomSounds setting at the AudioSource's position.
    /// </summary>
    public void PlayShotSound()
    {
        if (useRandomSounds && randomShotSounds != null && randomShotSounds.Length > 0)
        {
            int index = Random.Range(0, randomShotSounds.Length);
            audioSource.PlayOneShot(randomShotSounds[index], randomShotVolume);
        }
        else if (singleShotSound != null)
        {
            audioSource.PlayOneShot(singleShotSound, singleShotVolume);
        }
    }
}
}