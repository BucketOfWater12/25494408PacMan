using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip introMusic;
    [SerializeField] private AudioClip ghostNormalMusic;
    void Start()
    {
        audioSource.clip = introMusic;
        audioSource.loop = false;
        audioSource.Play();

        float delay = Mathf.Min(introMusic.length, 2.82f);
        Invoke(nameof(PlayGhostNormalMusic), delay);
    }
    void PlayGhostNormalMusic()
    {
        audioSource.clip = ghostNormalMusic;
        audioSource.loop = true;
        audioSource.Play();
    }
}
