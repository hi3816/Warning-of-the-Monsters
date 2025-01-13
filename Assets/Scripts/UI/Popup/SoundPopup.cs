using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundPopup : OptionPanelController
{
    private float _onAlpha = 255f / 255f;
    private float _offAlpha = 100f / 255f;

    public override void ClickBgmButton()
    {
        _soundManager.ChangeIsPlayBGM();

        if (_soundManager.IsPlayBGM)
        {
            _soundManager.PlayBGM(BgmType.Lobby);
        }
        else
        {
            _soundManager.StopBGM();
        }
        ChangeBgmImage();
    }

    protected override void ChangeBgmImage()
    {
        Color color = _bgmImage.color;
        color.a = _soundManager.IsPlayBGM ? _onAlpha : _offAlpha;
        _bgmImage.color = color;

        _bgmSlider.interactable = _soundManager.IsPlayBGM ? true : false;
    }

    protected override void ChangeSfxImage()
    {
        Color color = _sfxImage.color;
        color.a = _soundManager.IsPlaySFX ? _onAlpha : _offAlpha;
        _sfxImage.color = color;

        _sfxSlider.interactable = _soundManager.IsPlaySFX ? true : false;
    }
}
