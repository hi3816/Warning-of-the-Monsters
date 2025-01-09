using UnityEngine;
using UnityEngine.UI;

public class HandleGuidePages : UIBase
{
    [Header("Pages")]
    [SerializeField] private GameObject[] pages;

    [Header("Buttons")]
    [SerializeField] private Button _bookmark1Button;
    [SerializeField] private Button _bookmark2LeftButton;
    [SerializeField] private Button _bookmark2RightButton;
    [SerializeField] private Button _bookmark3LeftButton;
    [SerializeField] private Button _bookmark3RightButton;
    [SerializeField] private Button _bookmark4Button;

    private int _currentPage = 0;

    void Start()
    {
        _bookmark1Button.onClick.AddListener(() => ShowPage(0));
        _bookmark2LeftButton.onClick.AddListener(() => ShowPage(1));
        _bookmark2RightButton.onClick.AddListener(() => ShowPage(1));
        _bookmark3LeftButton.onClick.AddListener(() => ShowPage(2));
        _bookmark3RightButton.onClick.AddListener(() => ShowPage(2));
        _bookmark4Button.onClick.AddListener(() => ShowPage(3));
        ShowPage(0);
    }

    private void ShowPage(int pageIndex)
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(i == pageIndex);
        }

        _currentPage = pageIndex;
        UpdateButtonVisibility();
    }

    private void UpdateButtonVisibility()
    {
        if (_currentPage == 2) //page3
        {
            _bookmark1Button.gameObject.SetActive(true);
            _bookmark2LeftButton.gameObject.SetActive(true);
            _bookmark2RightButton.gameObject.SetActive(false);
            _bookmark3LeftButton.gameObject.SetActive(false);
            _bookmark3RightButton.gameObject.SetActive(true);
            _bookmark4Button.gameObject.SetActive(true);
        }
        else if (_currentPage == 3) //page4
        {
            _bookmark1Button.gameObject.SetActive(true);
            _bookmark2LeftButton.gameObject.SetActive(true);
            _bookmark2RightButton.gameObject.SetActive(false);
            _bookmark3LeftButton.gameObject.SetActive(true);
            _bookmark3RightButton.gameObject.SetActive(false);
            _bookmark4Button.gameObject.SetActive(true);
        }
        else //page 1, 2
        {
            _bookmark1Button.gameObject.SetActive(true);
            _bookmark2LeftButton.gameObject.SetActive(false);
            _bookmark2RightButton.gameObject.SetActive(true);
            _bookmark3LeftButton.gameObject.SetActive(false);
            _bookmark3RightButton.gameObject.SetActive(true);
            _bookmark4Button.gameObject.SetActive(true);
        }
    }
}