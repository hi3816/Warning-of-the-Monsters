using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBase : MonoBehaviour
{
    public Canvas canvas;

    public virtual void Opened(params object[] param)
    {

    }

    public void Hide(string uiName)
    {
        UIManager.Instance.Hide(gameObject.name);
    }
}
