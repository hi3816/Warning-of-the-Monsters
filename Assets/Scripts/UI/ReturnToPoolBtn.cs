using System;
using UnityEngine;

public class ReturnToPoolBtn : MonoBehaviour
{
    public static Action OnGameEnd;

    public void OnReturnBtnClicked()
    {
        OnGameEnd?.Invoke();
    }
}
