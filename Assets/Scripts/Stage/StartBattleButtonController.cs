using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartBattleButtonController : MonoBehaviour
{
    [SerializeField] private Image _timeCircleImage;
    [SerializeField] private HumanSpawner _humanSpawner;

    private bool _isWaveStart = true; // 웨이브가 시작했는지 확인
    private float _waveStartDelay; // 현재 웨이브 시작하기 전 지연 시간
    private float _currWaveStartDelayTime; // 현재 웨이브 지연 시간 설정
    private float _interWaveDelay; // 웨이브 간 지연 시간

    private Coroutine _interWaveCoroutine; // 웨이브간 지연 시간 계산 코루틴
    private WaitForSeconds _coroutineInterSeconds; // 웨이브간 지연 시간 캐싱 필드

    private Button _button;
    private Image[] _images;

    private void Awake()
    {
        _waveStartDelay = StageManager.Instance.StageData.waveStartDelay;
        _currWaveStartDelayTime = 0f;
        _interWaveDelay = StageManager.Instance.StageData.interWaveDelay;
        _coroutineInterSeconds = new WaitForSeconds(_interWaveDelay);
        
        _button = GetComponent<Button>();
        _images = GetComponentsInChildren<Image>();
        
        if (_humanSpawner == null)
            _humanSpawner = FindObjectOfType<HumanSpawner>();
    }

    private void Update()
    {
        if (!_isWaveStart)
        {
            _currWaveStartDelayTime += Time.deltaTime;
            ChangeStartBattleBtn();

            if(_currWaveStartDelayTime >= _waveStartDelay)
            {
                StartWave();
            }
        }
    }

    // StartBattleBtn 클릭
    public void StartWave()
    {
        if (HumanManager.Instance.isLastWave) return;
        
        _isWaveStart = true;
        _currWaveStartDelayTime = 0f;
        _timeCircleImage.fillAmount = 0f;
        ButtonDisable();

        if (!CheckClear())
        {
            if (_interWaveCoroutine != null) StopCoroutine(_interWaveCoroutine);
            _interWaveCoroutine = StartCoroutine(CoInterWaveDelay());
            StageManager.Instance.UpdateWave();
        }
        
        _humanSpawner.StartSpawningHumans(StageManager.Instance.CurrWave );
    }

    // 다음 웨이브 시간 계산
    private IEnumerator CoInterWaveDelay()
    {
        yield return _coroutineInterSeconds;
        _isWaveStart = false;
        if (!HumanManager.Instance.isLastWave)
            ButtonEnable();
    }

    // StartBattleBtn 이미지 변화
    private void ChangeStartBattleBtn()
    {
        _timeCircleImage.fillAmount = _currWaveStartDelayTime / _waveStartDelay;
    }

    // 웨이브가 끝났을 때
    public void EndWave()
    {
        if (!CheckClear() && !_button.enabled && !HumanManager.Instance.isLastWave)
        {
            ButtonEnable();
            if(_interWaveCoroutine != null) StopCoroutine(_interWaveCoroutine);
            _isWaveStart = false;
        }   
    }

    // 현재 스테이지를 클리어 했는지 확인
    private bool CheckClear()
    {
        if (StageManager.Instance.CheckEndStage())
        {
            // 클리어 조건 추가
            // Time.timeScale = 0f;
            SoundManager.Instance.StopBGM();
            return true;
            // 클리어 팝업 추가
        }
        return false;
    }

    private void ButtonEnable()
    {
        _button.enabled = true;
        foreach (Image image in _images)
        {
            image.enabled = true;
        }
    }

    private void ButtonDisable()
    {
        _button.enabled = false;
        foreach (Image image in _images)
        {
            image.enabled = false;
        }
    }
}