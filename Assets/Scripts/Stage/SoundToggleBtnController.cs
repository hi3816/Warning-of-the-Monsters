using UnityEngine;
using UnityEngine.UI;

public class SoundToggleBtnController : MonoBehaviour
{
    private SoundManager soundManager;

    [SerializeField] private SoundType soundType;
    [SerializeField] private Toggle toggle;
    
    private void Awake()
    {
        soundManager = SoundManager.Instance;
        //setToggleIsOn();
    }

    // SoundManager에 있는 초기값과 동기화
    private void setToggleIsOn()
    {
        switch (soundType)
        {
            case SoundType.BGM:
                {
                    toggle.isOn = soundManager.IsPlayBGM;
                    break;
                }
            case SoundType.SFX:
                {
                    toggle.isOn = soundManager.IsPlaySFX;
                    break;
                }
        }
    }

    // BGM 버튼 클릭 (On / Off)
    public void ClickBgmButton()
    {
        soundManager.ChangeIsPlayBGM();

        if (soundManager.IsPlayBGM)
        {
            soundManager.PlayBGM(BgmType.Stage);
        }
        else
        {
            soundManager.StopBGM();
        }
    }

    // SFX 버튼 클릭 (On / Off)
    public void ClickSfxButton()
    {
        soundManager.ChangeIsPlaySFX();
    }
}
