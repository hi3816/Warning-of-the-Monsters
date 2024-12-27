using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBase : MonoBehaviour
{
    public Canvas canvas;
    public enum UILayerOrder
    {
        Background = 3,
        Popup = 4,
        Topmost = 5
    }

    public virtual void Opened(params object[] param)
    {

    }

    public void Hide(string uiName)
    {
        UIManager.Instance.Hide(gameObject.name);
    }

    public void SetSortOrder(UILayerOrder order) 
    {
        Canvas canavas = GetComponentInParent<Canvas>();
        if (canavas == null)
        {
            Debug.Log("No Canvas found in parent hierarchy.");
            return;
        }
        canvas.sortingOrder = (int)order;
    }
}
