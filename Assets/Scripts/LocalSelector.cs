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
        
        if (_localID < 0 || _localID >= LocalizationSettings.AvailableLocales.Locales.Count)
        {
            active = false;
            yield break;
        }

        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localID];
        PlayerPrefs.SetInt("LocalKey", _localID);
        PlayerPrefs.Save();
        active = false;
    }
}
