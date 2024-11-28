using UnityEngine;

public class SoundManager : SingletonBase<SoundManager>
{

    [SerializeField] AudioClip[] bgms;
    [SerializeField] AudioClip[] sfxs;

    [SerializeField] AudioSource audioBgm;
    [SerializeField] AudioSource audioSfx;

    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(this);
    }

    // 배경 음악 재생
    public void PlayBGM(BgmType bgmType)
    {
        // enum -> int 로 형변환
        audioBgm.clip = bgms[(int)bgmType];
        audioBgm.Play();
    }

    // 배경 음악 정지
    public void StopBGM()
    {
        audioBgm.Stop();
    }

    // 효과음 재생
    public void PlaySFX(SfxType sfxType)
    {
        // enum -> int 로 형변환
        audioSfx.PlayOneShot(sfxs[(int)sfxType]);
    }
}
