using UnityEngine;

public class StartScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UIManager.Instance.Show<StartScreen>();
        SoundManager.Instance.StopBGM();
        SoundManager.Instance.PlayBGM(BgmType.Intro);
    }
}
