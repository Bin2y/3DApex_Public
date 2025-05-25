using UnityEngine;

public class DummyFootstepHandler : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] walkClips;

    private void OnEnable()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void PlayWalkFootstep()
    {
        PlayRandom(walkClips);
    }
    public void PlayRunFootstep()
    {
        PlayRandom(walkClips);
    }

    private void PlayRandom(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return;
        var clip = clips[Random.Range(0, clips.Length)];
        audioSource.PlayOneShot(clip);
    }
}
