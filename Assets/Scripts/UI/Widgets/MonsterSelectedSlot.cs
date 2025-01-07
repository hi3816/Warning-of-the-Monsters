using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterSelectedSlot : MonoBehaviour
{
    public Image slotImg;
    public Image arrowImg;

    public void UpdateSelectedSlot(Sprite listSlotSprite)
    {
        slotImg.sprite = listSlotSprite;
        slotImg.color = new Color(1, 1, 1, 1);
    }
}
