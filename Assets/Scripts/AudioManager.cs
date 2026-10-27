using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip introMusic;
    [SerializeField] private AudioClip ghostNormalMusic;
    [SerializeField] private AudioClip ghostScaredMusic;
    [SerializeField] private AudioClip ghostDeadMusic;

    public void PlayIntro()
    {
        PlayClip(introMusic, false);
    }

    public void PlayNormal()
    {
        PlayClip(ghostNormalMusic, true);
    }

    public void PlayScared()
    {
        PlayClip(ghostScaredMusic, true);
    }

    public void PlayDead()
    {
        PlayClip(ghostDeadMusic, true);
    }

    public void StopMusic()
    {
        CancelInvoke();
        audioSource.Stop();
    }

    void PlayClip(AudioClip clip, bool loop)
    {
        if (audioSource.clip == clip && audioSource.isPlaying) return;

        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.Play();
    }
}
