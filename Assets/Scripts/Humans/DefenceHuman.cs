public class DefenceHuman : Human
{
    private float _defenceChance;
    
    protected override void Awake()
    {
        base.Awake();
        _defenceChance = controller.Cooldown;
    }

    public override void IncreaseFear(float amount)
    {
        float randNum = UnityEngine.Random.Range(0f, 10f);
        if (randNum < _defenceChance)   // (쿨타임 * 10)% 의 확률로 공격을 무효화
        {
            // 방어 성공 시 애니메이션 실행 후 종료
            controller.animator.SetTrigger("Defence");
            return;
        }
        // 방어 실패 시 공포 수치 증가
        base.IncreaseFear(amount);
    }
}