using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartBattleButtonController : MonoBehaviour
{
    [SerializeField] private Image timeCircleImage;

    private bool isWaveStart = true; // 웨이브가 시작했는지 확인
    private float waveStartDelay; // 현재 웨이브 시작하기 전 지연 시간
    private float currWaveStartDelayTime; // 현재 웨이브 지연 시간 설정
    private float interWaveDelay; // 웨이브 간 지연 시간

    private Coroutine coroutine; // 웨이브간 지연 시간 계산 코루틴
    private WaitForSeconds coroutineInterSeconds; // 웨이브간 지연 시간 캐싱 필드

    [SerializeField] private Button button;
    [SerializeField] private Image[] images;

    private void Awake()
    {
        waveStartDelay = StageManager.Instance.stageSO.waveStartDelay;
        currWaveStartDelayTime = 0f;
        interWaveDelay = StageManager.Instance.stageSO.interWaveDelay;
        coroutineInterSeconds = new WaitForSeconds(interWaveDelay);
        
        button = GetComponent<Button>();
        images = GetComponentsInChildren<Image>();
    }

    private void Update()
    {
        if (!isWaveStart)
        {
            currWaveStartDelayTime += Time.deltaTime;
            ChangeStartBattleBtn();

            if(currWaveStartDelayTime >= waveStartDelay)
            {
                StartWave();
            }
        }
    }

    // StartBattleBtn 클릭
    public void StartWave()
    {
        isWaveStart = true;
        currWaveStartDelayTime = 0f;
        timeCircleImage.fillAmount = 0f;
        ButtonDisable();

        if (!CheckClear())
        {
            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = StartCoroutine(CoInterWaveDelay());
            StageManager.Instance.UpdateWave();
        }
    }

    // 다음 웨이브 시간 계산
    private IEnumerator CoInterWaveDelay()
    {
        yield return coroutineInterSeconds;
        isWaveStart = false;
        ButtonEnable();
    }

    // StartBattleBtn 이미지 변화
    private void ChangeStartBattleBtn()
    {
        timeCircleImage.fillAmount = currWaveStartDelayTime / waveStartDelay;
    }

    // 웨이브가 끝났을 때
    public void EndWave()
    {
        if (!CheckClear() && !button.enabled)
        {
            ButtonEnable();
            if(coroutine != null) StopCoroutine(coroutine);
            isWaveStart = false;
        }   
    }

    // 현재 스테이지를 클리어 했는지 확인
    private bool CheckClear()
    {
        if (StageManager.Instance.CheckEndStage())
        {
            // 클리어 조건 추가
            Time.timeScale = 0f;
            SoundManager.Instance.StopBGM();
            return true;
            // 클리어 팝업 추가
        }
        return false;
    }

    private void ButtonEnable()
    {
        button.enabled = true;
        foreach (Image image in images)
        {
            image.enabled = true;
        }
    }

    private void ButtonDisable()
    {
        button.enabled = false;
        foreach (Image image in images)
        {
            image.enabled = false;
        }
    }
}