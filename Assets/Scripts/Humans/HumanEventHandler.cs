using System;
using UnityEngine;

public class HumanEventHandler : MonoBehaviour
{
    [SerializeField] private HumanController _controller;

    private void OnEnable()
    {
        if (_controller == null)
            gameObject.GetComponentInParent<HumanController>();
        _controller.isSurprising = false;
    }
    
    public void SetSurprising()
    {
        _controller.isSurprising = true;
    }
    
    public void SetNotSurprising()
    {
        _controller.isSurprising = false;
    }
}
