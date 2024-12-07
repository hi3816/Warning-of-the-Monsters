using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Human : MonoBehaviour
{
    public HumanSO humanData;

    public Image fearGauge;

    public bool IsWaveStarted { get; set; }
    public float FearLevel { get; set; }
    public int WaveIdx { get; set; }

    private void Awake()
    {
        // 리소스 폴더에서 SO 데이터 로드
        humanData = CustomUtil.ResourceLoad<HumanSO>("SO/Human/HumanSO_0");
    }

    private void OnEnable()
    {
        IsWaveStarted = true;
        FearLevel = 0;
        fearGauge.fillAmount = 0;
    }

    public bool IsFearMaxed()
    {
        return FearLevel >= humanData.maxFear;
    }

    public void IncreaseFear(float fearInflicted)
    {
        if (IsFearMaxed()) return;
        
        FearLevel += math.min(FearLevel + fearInflicted, humanData.maxFear);
        fearGauge.fillAmount = FearLevel / humanData.maxFear;
    }
}