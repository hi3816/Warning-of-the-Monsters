using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageButton : MonoBehaviour
{
    [SerializeField] GameObject _stageButton;
    [SerializeField] TextMeshProUGUI _stageTxt;
    [SerializeField] Sprite _isClearImg;

    public int stageIdx;
    public bool isCleared;
    public bool isEnable;

    void Start()
    {
        _stageTxt.text = _stageButton.name;

        stageIdx = int.Parse(Regex.Match(_stageTxt.text, @"\d+").Value)-1;
        
        SaveManager.Instance.GetStagePlayInfo(stageIdx, out isCleared);

        if (isCleared)
        {
            _stageTxt.text = "Clear";
            Image btnImg = _stageButton.GetComponent<Image>();
            btnImg.sprite = _isClearImg;
        }

        bool previousClear = false ;
        SaveManager.Instance.GetStagePlayInfo(stageIdx -1 , out previousClear);
        if (previousClear == true) isEnable = true;

        if (stageIdx > 7)
        {
            isCleared = false;
            isEnable = false; // 스테이지 8까지만 열리도록 제한
        }
        
        if (!isEnable)
        {
            _stageButton.GetComponent<Button>().enabled = false;

            _stageTxt.text = "";
            Image btnImg = _stageButton.GetComponent<Image>();
            btnImg.color = Color.gray;
        }
    }

}
