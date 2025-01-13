using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocalSelector : MonoBehaviour
{
    private bool active = false;

    private void Start()
    {
        int ID = PlayerPrefs.GetInt("LocalKey", 1);
        ChangeLocal(ID);
    }

    public void ChangeLocal(int localeID)
    {
        if (active) return;
        StartCoroutine(SetLocale(localeID));
    }
    
    IEnumerator SetLocale(int _localID)
    {
        active = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localID];
        PlayerPrefs.SetInt("LocalKey", _localID);
        active = false;
    }
}
