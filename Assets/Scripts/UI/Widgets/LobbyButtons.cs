using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyButtons : MonoBehaviour
{
    [SerializeField] private GameObject _soundOptionPanel;
    [SerializeField] private TextMeshProUGUI startCountTxt;
    private int starCount;

    private void Start()
    {
        SaveManager.Instance.GetStarCount(out starCount);
        startCountTxt.text = $"{starCount.ToString()} / 24";
    }

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
        LoadingManager.Instance.ChangeScene("TitleScene");
    }
}
