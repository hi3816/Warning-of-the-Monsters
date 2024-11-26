public class GameManager : SingletonBase<GameManager>
{
    protected override void Awake()
    {
        base.Awake();
        
        // 매니저 중 DontDestroyOnLoad 인 인스턴스만 주석 활성화하여 사용
        // DontDestroyOnLoad(this);
    }
}
