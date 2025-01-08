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
    [SerializeField] private Button _bookmark3Button;

    private int _currentPage = 0;

    void Start()
    {
        _bookmark1Button.onClick.AddListener(() => ShowPage(0));
        _bookmark2LeftButton.onClick.AddListener(() => ShowPage(1));
        _bookmark2RightButton.onClick.AddListener(() => ShowPage(1));
        _bookmark3Button.onClick.AddListener(() => ShowPage(2));
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
        if (_currentPage == 2)
        {
            _bookmark1Button.gameObject.SetActive(true);
            _bookmark2LeftButton.gameObject.SetActive(true);
            _bookmark2RightButton.gameObject.SetActive(false);
            _bookmark3Button.gameObject.SetActive(true);
        }
        else
        {
            _bookmark1Button.gameObject.SetActive(true);
            _bookmark2LeftButton.gameObject.SetActive(false);
            _bookmark2RightButton.gameObject.SetActive(true);
            _bookmark3Button.gameObject.SetActive(true);
        }
    }
}