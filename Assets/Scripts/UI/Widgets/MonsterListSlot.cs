using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class MonsterListSlot : MonoBehaviour
{
    [SerializeField] Image slotSprite;

    public void SelectListSlot()
    {
        Sprite sprite = slotSprite.sprite;

        UIManager.Instance.OnClickListSlot?.Invoke(sprite);
    }
}
