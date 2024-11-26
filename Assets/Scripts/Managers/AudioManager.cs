using UnityEngine;

public class AudioManager : SingletonBase<AudioManager>
{
    
    
    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(this);
    }
}
