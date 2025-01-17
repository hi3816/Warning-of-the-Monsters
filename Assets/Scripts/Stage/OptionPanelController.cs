using UnityEngine;
using UnityEngine.UI;

public class OptionPanelController : MonoBehaviour
{
    [Header("ToggleButton")]
    [SerializeField] protected Image _bgmImage;
    [SerializeField] protected Image _sfxImage;

    [Header("Slider")]
    [SerializeField] protected Slider _sfxSlider;
    [SerializeField] protected Slider _bgmSlider;

    private float _onAlpha = 255f / 255f;
    private float _offAlpha = 200f / 255f;
    private int _timeScale;
    protected SoundManager _soundManager;

    private void Awake()
    {
        _soundManager = SoundManager.Instance;
        SetMusicButton();
    }

    private void Start()
    {
        if (SoundManager.Instance != null)
        {
            _sfxSlider.value = SoundManager.Instance.globalSfxVolume;
            _bgmSlider.value = SoundManager.Instance.bgmVolume;

            // 슬라이더 값 변경 시 사운드 매니저의 볼륨 조절
            _sfxSlider.onValueChanged.AddListener((value) =>
            {
                SoundManager.Instance.SetSfxVolume(value);
            });

            _bgmSlider.onValueChanged.AddListener((value) =>
            {
                SoundManager.Instance.SetBgmVolume(value);
            });
        }
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
    public virtual void ClickBgmButton()
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
    protected virtual void ChangeBgmImage()
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
    protected virtual void ChangeSfxImage()
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
        LoadingManager.Instance.ChangeScene("MainScene");
    }

    // Exit 버튼 클릭
    public void ClickExitButton()
    {
        PoolManager.Instance.DeleteAllPools();
        PoolManager.Instance.AddPools<SfxSoundSource>(SoundManager.Instance.poolconfigs);
        gameObject.SetActive(false);
        Time.timeScale = 1f;
        LoadingManager.Instance.ChangeScene("LobbyScene");
    }

    // X 버튼 클릭
    public void ClickContinueButton()
    {
        gameObject.SetActive(false);
        Time.timeScale = _timeScale;
    }
}