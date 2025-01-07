using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyButtons : MonoBehaviour
{
    [SerializeField] private GameObject _soundOptionPanel;

    public void OnToggleOptionsPanel()
    {
        if (_soundOptionPanel.activeSelf)
        {
            _soundOptionPanel.SetActive(false);
        }
        else
        {
            _soundOptionPanel.SetActive(true);
        }
    }

    public void LoadTitleScene()
    {
        MySceneManager.Instance.ChangeScene("TitleScene");
    }
}
