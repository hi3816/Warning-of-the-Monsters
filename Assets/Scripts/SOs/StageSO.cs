using UnityEngine;

[CreateAssetMenu(fileName = "StageSO", menuName = "StageSO")]

public class StageSO : ScriptableObject
{
    public int wave; // 스테이지별 웨이브
    public int health; // 스테이지별 체력
    public int gold; // 스테이지별 기본 골드
    public float waveStartDelay; // 현재 웨이브의 시작하기 전 지연 시간
    public float interWaveDelay; // 웨이브 간의 지연 시간
}
