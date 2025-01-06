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
        _controller.isSurprising = true;    // Surprise 애니메이션 시작 시 놀람 상태로 설정
    }
    
    public void SetNotSurprising()
    {
        _controller.isSurprising = false;   // Surprise 애니메이션 종료 시 놀라지 않은 상태로 설정
    }
}
