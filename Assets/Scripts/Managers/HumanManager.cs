using System;

public enum HumanType
{
    NormalHuman,
    StrongHuman,
}
public class HumanManager : SingletonBase<HumanManager>
{
    private int _countPerWave;
    public int CountPerWave
    {
        get { return _countPerWave; }
        set
        {
            _countPerWave = value;
            //Debug.Log(CountPerWave);
        }
    }
    public bool isLastWave;
    
    private int _currentWave;
    private int _totalHumansInWave;
    private Wave_Data.Stage1 _waveData;
    
    public Action<int> OnWaveCleared;
    public Action OnGameClear;

    private void OnEnable()
    {
        isLastWave = false;
        CountPerWave = 0;
    }
    
    public void SubHumanCount()
    {
        CountPerWave--;
        if (CountPerWave <= 0 && isLastWave)
            OnGameClear?.Invoke();
    }

}