using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Human : MonoBehaviour
{
    private HumanSO humanData;
    public float FearLevel { get; private set; }
    public float MaxFear { get; private set; }
    public int LifeInflicted { get; private set; }
    public int Coin { get; private set; }

    public int WaveIdx { get; set; }

    public Image FearGauge;
    public HumanController controller;

    private void Awake()
    {
        humanData = CustomUtil.ResourceLoad<HumanSO>("SO/Human/HumanSO_0");
        controller = GetComponent<HumanController>();
        MaxFear = humanData.maxFear;
        LifeInflicted = humanData.lifeInflicted;
        Coin = humanData.coin;
    }

    private void OnEnable()
    {
        FearLevel = 0;
        FearGauge.fillAmount = 0;
    }

    public void IncreaseFear(float amount)
    {
        FearLevel = Mathf.Min(FearLevel + amount, MaxFear); 
        FearGauge.fillAmount = FearLevel / MaxFear;
        Debug.LogWarning($"Fear: {FearLevel}");
        if (FearLevel >= MaxFear)
        {
            controller.ReturnHumanToPool(3.0f);
            controller._stateMachine.ChangeState(controller.RunHumanState);
            StageManager.Instance.ChangeGold(Coin);
        }
    }

    public void ResetFear()
    {
        FearLevel = 0;
    }
}