using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterListSlot : MonoBehaviour
{
    [SerializeField]
    private Image slotImage;

    public void setSlotImage(Sprite monsterSprite)
    {
        slotImage.sprite = monsterSprite;
    }
}
