using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageButton : MonoBehaviour
{
    [SerializeField] GameObject stageButton;
    [SerializeField] TextMeshProUGUI stageTxt;
    [SerializeField] Sprite _isClearImg;

    public int stageIdx;
    public bool isCleared;
    public bool isEnable;

    void Start()
    {
        stageTxt.text = stageButton.name;

        stageIdx = int.Parse(Regex.Match(stageTxt.text, @"\d+").Value)-1;

        if (isCleared)                                                                                                                                                                          
        {
            stageTxt.text = "Clear";
            Image btnImg = stageButton.GetComponent<Image>();
            btnImg.sprite = _isClearImg;
        }
        
        if (!isEnable)
        {
            stageButton.GetComponent<Button>().enabled = false;

            stageTxt.text = "";
            Image btnImg = stageButton.GetComponent<Image>();
            btnImg.color = Color.gray;
        }
    }

}
