using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocalSelector : MonoBehaviour
{
    private bool _active = false;
    private Coroutine _coroutine; 

    private void Start()
    {
        int ID = PlayerPrefs.GetInt("LocalKey", 1);
        ChangeLocal(ID);
    }

    public void ChangeLocal(int localeID)
    {
        if (_active) return;
        if (_coroutine != null) StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(SetLocale(localeID));
    }
    
    IEnumerator SetLocale(int _localID)
    {
        _active = true;
        yield return LocalizationSettings.InitializationOperation;
        
        if (_localID < 0 || _localID >= LocalizationSettings.AvailableLocales.Locales.Count)
        {
            _active = false;
            yield break;
        }

        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localID];
        PlayerPrefs.SetInt("LocalKey", _localID);
        PlayerPrefs.Save();
        _active = false;
    }
}
