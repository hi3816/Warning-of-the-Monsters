using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionPanelController : MonoBehaviour
{
    [Header("ToggleButton")]
    [SerializeField] private Image bgmImage;
    [SerializeField] private Image sfxImage;

    private float onAlpha = 255f / 255f;
    private float offAlpha = 200f / 255f;

    private SoundManager soundManager;

    private void Awake()
    {
        soundManager = SoundManager.Instance;
        SetMusicButton();
    }

    // BGM, SFX 세팅
    public void SetMusicButton()
    {
        ChangeBgmImage();
        ChangeSfxImage();
    }

    // BGM 버튼 클릭 (On / Off)
    public void ClickBgmButton()
    {
        soundManager.ChangeIsPlayBGM();

        if (soundManager.IsPlayBGM)
        {
            soundManager.PlayBGM(BgmType.Stage);
        }
        else
        {
            soundManager.StopBGM();
        }
        ChangeBgmImage();
    }

    // BGM On Off 변경시 이미지 alpha 값 변경
    private void ChangeBgmImage()
    {
        Color color = bgmImage.color;
        color.a = soundManager.IsPlayBGM ? onAlpha : offAlpha;
        bgmImage.color = color;
    }

    // SFX 버튼 클릭 (On / Off)
    public void ClickSfxButton()
    {
        soundManager.ChangeIsPlaySFX();
        ChangeSfxImage();
    }

    // SFX On Off 변경시 SFX 이미지 alpha 값 변경
    private void ChangeSfxImage()
    {
        Color color = sfxImage.color;
        color.a = soundManager.IsPlaySFX ? onAlpha : offAlpha;
        sfxImage.color = color;
    }

    // Retry 버튼 클릭
    private void ClickRetryButton()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
        SceneManager.LoadScene("StageScene");
    }
}
