using UnityEngine;

public class SfxSoundSource : MonoBehaviour
{
    private AudioSource audioSource;
    private SfxType type;

    public void Play(AudioClip clip, SfxType sfxType, float soundEffectVolume, float soundEffectPitchVaricance)
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        CancelInvoke();
        type = sfxType;
        audioSource.clip = clip;
        audioSource.volume = soundEffectVolume;
        audioSource.pitch = 1f + Random.Range(-soundEffectPitchVaricance, soundEffectPitchVaricance); // 다양한 음향 효과
        audioSource.PlayOneShot(clip);

        Invoke("Disable", clip.length * 2); // clip.length -> 오디오 클립의 재생시간
    }

    public void Disable()
    {
        if(audioSource.isPlaying) audioSource.Stop();
        PoolManager.Instance.ReturnToPool(type.ToString(), gameObject);
    }
}