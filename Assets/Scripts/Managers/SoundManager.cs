using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SoundManager : SingletonBase<SoundManager>
{
    [Header("BGM List")]
    Dictionary<BgmType, AudioClip> bgmDict = new Dictionary<BgmType, AudioClip>();
    
    [System.Serializable]
    private class BgmKeyValuePair
    {
        public BgmType bgmType;
        public AudioClip clip;
    }
    [SerializeField] private List<BgmKeyValuePair> bgmList;

    [Header("SFX List")]
    Dictionary<SfxType, AudioClip> sfxDict = new Dictionary<SfxType, AudioClip>();

    [System.Serializable]
    private class SfxKeyValuePair
    {
        public SfxType sfxType;
        public AudioClip clip;
    }
    [SerializeField] private List<SfxKeyValuePair> SfxList;

    private AudioSource audioBgm;
    
    // BGM 볼륨
    [SerializeField][Range(0f, 1f)] private float bgmVolume;

    // SFX 볼륨 및 음 높낮이 조절
    [SerializeField][Range(0f, 1f)] private float globalSfxVolume;
    [SerializeField][Range(0f, 1f)] private float sfxPitchVariance; // 높은 음이 나옴

    private Dictionary<SfxType, float> individualSfxVolumeDict;

    public bool IsPlayBGM { get; private set; } // BGM 출력 설정 (On / Off)
    public bool IsPlaySFX { get; private set; } // SFX 출력 설정 (On / Off)

    public PoolManager.PoolConfig[] poolconfigs;

    protected override void Awake()
    {
        audioBgm = GetComponent<AudioSource>();
        audioBgm.volume = bgmVolume;
        audioBgm.loop = true;
        IsPlayBGM = true;
        IsPlaySFX = true;

        SetBgmDictionary();
        SetSfxDictionary();
        SetSfxVolumeDictionary();
        PoolManager.Instance.AddPools(poolconfigs);
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnClick();
        }
    }

    private void SetBgmDictionary()
    {
        if (bgmList == null) return; 

        foreach(BgmKeyValuePair bgm in bgmList)
        {
            if (!bgmDict.ContainsKey(bgm.bgmType))
            {
                bgmDict.Add(bgm.bgmType, bgm.clip);
            }
        }
    }

    private void SetSfxDictionary()
    {
        if (SfxList == null) return;

        foreach (SfxKeyValuePair sfx in SfxList)
        {
            if (!sfxDict.ContainsKey(sfx.sfxType)) 
            {
                sfxDict.Add(sfx.sfxType, sfx.clip);
            }
        }
    }

    private void SetSfxVolumeDictionary()
    {
        individualSfxVolumeDict = DataManager.Instance.GetIndvidualSfxVolumeDict();
        
        foreach(SfxType key in sfxDict.Keys)
        {
            // 해당 키값이 없을 때 개별 볼륨을 1.0 으로 초기화
            // 즉 전체 Sfx 볼륨과 일치
            if (!individualSfxVolumeDict.ContainsKey(key))
            {
                individualSfxVolumeDict.Add(key, 1.0f);
            }
        }
    }

    // 배경 음악 시작
    public void PlayBGM(BgmType bgmType)
    {
        if (IsPlayBGM)
        {
            audioBgm.clip = bgmDict[bgmType];
            audioBgm.mute = false;
            audioBgm.loop = true;
            audioBgm.Play();
        }
    }

    // 배경 음악 정지
    public void StopBGM()
    {
        audioBgm.Stop();
    }

    // 효과음 재생
    public void PlaySFX(SfxType sfxType)
    {
        if (IsPlaySFX)
        {
            GameObject obj = PoolManager.Instance.SpawnFromPool(sfxType.ToString());
            obj.SetActive(true);
            SfxSoundSource soundSource = obj.GetComponent<SfxSoundSource>();
            soundSource.Play(sfxDict[sfxType], sfxType, globalSfxVolume * individualSfxVolumeDict[sfxType], sfxPitchVariance);
        }
    }

    // BGM On / Off
    public void ChangeIsPlayBGM()
    {
        IsPlayBGM = !IsPlayBGM;
    }

    // SFX On / Off
    public void ChangeIsPlaySFX()
    {
        IsPlaySFX = !IsPlaySFX;
    }

    public void OnClick()
    {
        // IsPointerOverGameObject() -> UI만 작동하도록 제어
        if(EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            PlaySFX(SfxType.Click);
        }
    }
}
