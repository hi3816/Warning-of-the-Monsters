using System.Collections;
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

        StartCoroutine(CoDisable(clip.length));
    }

    private IEnumerator CoDisable(float length)
    {
        yield return new WaitForSecondsRealtime(length);
        gameObject.SetActive(false);
        PoolManager.Instance.ReturnToPool(type.ToString(), gameObject);
    }
}