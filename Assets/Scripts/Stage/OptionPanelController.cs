using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionPanelController : MonoBehaviour
{
    [Header("ToggleButton")]
    [SerializeField] private Image _bgmImage;
    [SerializeField] private Image _sfxImage;

    private float _onAlpha = 255f / 255f;
    private float _offAlpha = 200f / 255f;

    private SoundManager _soundManager;
    private int _timeScale;

    private void Awake()
    {
        _soundManager = SoundManager.Instance;
        SetMusicButton();
    }

    private void OnEnable()
    {
        _timeScale = Mathf.RoundToInt(Time.timeScale);
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
        _soundManager.ChangeIsPlayBGM();

        if (_soundManager.IsPlayBGM)
        {
            _soundManager.PlayBGM(BgmType.Stage);
        }
        else
        {
            _soundManager.StopBGM();
        }
        ChangeBgmImage();
    }

    // BGM On Off 변경시 이미지 alpha 값 변경
    private void ChangeBgmImage()
    {
        Color color = _bgmImage.color;
        color.a = _soundManager.IsPlayBGM ? _onAlpha : _offAlpha;
        _bgmImage.color = color;
    }

    // SFX 버튼 클릭 (On / Off)
    public void ClickSfxButton()
    {
        _soundManager.ChangeIsPlaySFX();
        ChangeSfxImage();
    }

    // SFX On Off 변경시 SFX 이미지 alpha 값 변경
    private void ChangeSfxImage()
    {
        Color color = _sfxImage.color;
        color.a = _soundManager.IsPlaySFX ? _onAlpha : _offAlpha;
        _sfxImage.color = color;
    }

    // Retry 버튼 클릭
    public void ClickRetryButton()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainScene");
    }

    // Exit 버튼 클릭
    public void ClickExitButton()
    {
        PoolManager.Instance.DeleteAllPools();
        PoolManager.Instance.AddPoolS(SoundManager.Instance.poolconfigs);
        gameObject.SetActive(false);
        Time.timeScale = 1f;
        SceneManager.LoadScene("LobbyScene");
    }

    // X 버튼 클릭
    public void ClickContinueButton()
    {
        gameObject.SetActive(false);
        Time.timeScale = _timeScale;
    }
}