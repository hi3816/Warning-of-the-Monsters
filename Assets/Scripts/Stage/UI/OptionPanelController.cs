using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionPanelController : MonoBehaviour
{
    [Header("ToggleButton")]
    [SerializeField] private Toggle bgmToggle;
    [SerializeField] private Toggle sfxToggle;

    private SoundManager soundManager;

    private void Awake()
    {
        soundManager = SoundManager.Instance;
        SetToggleIsOn();
    }

    // ToggleButton 초기화
    private void SetToggleIsOn()
    {
        bgmToggle.isOn = soundManager.IsPlayBGM;
        sfxToggle.isOn = soundManager.IsPlaySFX;
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
    }

    // SFX 버튼 클릭 (On / Off)
    public void ClickSfxButton()
    {
        soundManager.ChangeIsPlaySFX();
    }

    // Retry 버튼 클릭
    public void ClickRetryButton()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
        SceneManager.LoadScene("StageScene");
    }
}
