using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningBox : UIBase
{
    [SerializeField] private CanvasGroup _warningGroup;
    [SerializeField] private Transform _warningTransform;
    
    private void OnEnable()
    {
        _warningGroup.DOFade(1f, 1f);

        _warningTransform.DOLocalMove(new Vector3(0, 10, 0), 0.5f)
            .SetRelative();
    }
}
