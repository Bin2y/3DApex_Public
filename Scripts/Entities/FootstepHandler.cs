using UnityEngine;

public class FootstepHandler : MonoBehaviour
{
    [Header("발소리 설정")]
    [Tooltip("걸음 소리 클립 리스트")]
    public AudioClip[] walkClips;
    public AudioClip[] runClips;
    public AudioClip[] jumpEndClips;

    [Header("Components")]
    public AudioSource audioSource;
    public Animator animator;
    private FPSMovement fpsMovement;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        fpsMovement = GetComponent<FPSMovement>();
    }

    public void PlayWalkFootstep()
    {
        if (!fpsMovement.IsMoving()) return;
        PlayRandom(walkClips);
    }

    public void PlayRunFootstep()
    {
        if (!fpsMovement.IsMoving()) return;
        PlayRandom(runClips);
    }

    public void PlayJumpEnd()
    {
        PlayRandom(runClips);
    }

    //랜덤으로 선택해서 재생
    private void PlayRandom(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return;
        var clip = clips[Random.Range(0, clips.Length)];
        audioSource.PlayOneShot(clip);
    }
}
