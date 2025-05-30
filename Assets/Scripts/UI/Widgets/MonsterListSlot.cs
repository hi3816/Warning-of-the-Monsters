using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class MonsterListSlot : MonoBehaviour
{
    [SerializeField] Image _slotSprite;

    public void SelectListSlot()
    {
        Sprite sprite = _slotSprite.sprite;

        UIManager.Instance.OnClickListSlot?.Invoke(sprite);
    }
}
