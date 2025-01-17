using System;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToPoolBtn : MonoBehaviour
{
    public void OnReturnBtnClicked()
    {
        GameManager.Instance.ReturnObjects();
    }
}
