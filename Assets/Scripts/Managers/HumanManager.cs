using System;
using System.Collections.Generic;

public enum HumanType
{
    NormalHuman,
    StrongHuman,
    SuperHuman,
    MysticHuman,
}
public class HumanManager : SingletonBase<HumanManager>
{
    // 웨이브별 현재 존재하는 인간 수를 관리하는 딕셔너리
    // key: 스폰된 웨이브 인덱스, value: 해당 웨이브에 스폰되어 현재 존재하는 인간 수
    public Dictionary<int, int> countPerWave = new Dictionary<int, int>();

    public bool isLastWave;
    
    private int _currentWave;
    private int _totalHumansInWave;
    
    private void OnEnable()
    {
        isLastWave = false;
        countPerWave.Clear();
    }
    
    // 풀에 인간이 반환될 때 인원 수 관리하는 딕셔너리에서 차감
    public void SubHumanCount(int spawnedIdx)
    {
        if (countPerWave.ContainsKey(spawnedIdx))   // 스폰된 인덱스 키 값 있으면
        {
            countPerWave[spawnedIdx]--; // 해당 키값의 인원 수 차감
            if (countPerWave[spawnedIdx] == 0)  // 해당 웨이브에서 스폰된 모든 인원이 없어지면
            {
                countPerWave.Remove(spawnedIdx);    // 딕셔너리에서 제거
                StageManager.Instance.ClickEndWaveBtn();    // 웨이브를 종료하고 새로운 웨이브 활성화
            }
        }
        
        if (countPerWave.Count == 0 && isLastWave)  // 딕셔너리에 값이 없고(모든 인간 처치) 마지막 웨이브면
            GameManager.Instance.GameClear();
    }
}