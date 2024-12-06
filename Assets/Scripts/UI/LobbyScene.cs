using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyScene : MonoBehaviour
{
    private void Awake()
    {
        //로비 시작부분
        SoundManager.Instance.StopBGM();
        SoundManager.Instance.PlayBGM(BgmType.Lobby);
    }
}
