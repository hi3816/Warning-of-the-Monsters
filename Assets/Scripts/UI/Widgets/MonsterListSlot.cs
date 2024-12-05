using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class MonsterListSlot : MonoBehaviour
{

    public void SelectListSlot()
    {
        Sprite sprite = transform.GetChild(0).GetComponent<Image>().sprite;

        UIManager.Instance.OnClickListSlot?.Invoke(sprite);
    }
}
