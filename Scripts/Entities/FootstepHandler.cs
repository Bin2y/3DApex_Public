using UnityEngine;

public class FootstepHandler : MonoBehaviour
{
    [Header("�߼Ҹ� ����")]
    [Tooltip("���� �Ҹ� Ŭ�� ����Ʈ")]
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

    //�������� �����ؼ� ���
    private void PlayRandom(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return;
        var clip = clips[Random.Range(0, clips.Length)];
        audioSource.PlayOneShot(clip);
    }
}
