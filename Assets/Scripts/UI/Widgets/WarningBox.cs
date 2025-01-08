using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WarningBox : UIBase
{
    [SerializeField] private CanvasGroup _warningGroup;
    [SerializeField] private Transform _warningTransform;

    [SerializeField] private TextMeshProUGUI _text;

    private void OnEnable()
    {
        transform.SetAsLastSibling();

        _warningGroup.DOFade(1f, 0.5f)
            .OnStart(() => 
            {
                _warningTransform.localPosition = Vector3.zero;
            })
            .OnComplete(() =>
            {
                DOVirtual.DelayedCall(1f, () => FadeOut());
            });

            _warningTransform.DOLocalMove(new Vector3(0, 50, 0), 0.5f)
            .SetRelative();
    }

    private void FadeOut()
    {
        _warningGroup.DOFade(0f, 0.5f)
            .OnStart(() => 
            {
                _warningTransform.DOLocalMove(new Vector3(0, 100, 0), 0.5f);
            })
            .OnComplete(() =>
            {
                PoolManager.Instance.ReturnToPool(this.name, this);
            });
    }

    public void SetText(string text)
    {
        _text.text = text;
    }
}
