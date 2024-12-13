using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterEvolutionUI : MonoBehaviour
{

    [System.Serializable]
    private class EvolutionSprite
    {
        public int monsterID; // 2 드래곤 3 리치 4 사이클롭스
        public EvolutionType[] evolutionType;
        public Sprite[] sprite;
    }

    // Inspector에서 Sprite 넣기 위해 사용하는 List
    [SerializeField] private List<EvolutionSprite> evolutionSpritePairList;
    [SerializeField] private Image[] typeImages;
    [SerializeField] private GameObject evolutionUI;

    private Dictionary<int, (EvolutionType[], Sprite[])> evolutionSpriteDict = new Dictionary<int, (EvolutionType[], Sprite[])>(); // 스킬 스프라이트 담을 Dict

    private Monster selectMonster;

    private void Awake()
    {
        SetSprite();
    }

    public void Show(Monster monster)
    {
        selectMonster = monster;
        Vector3 worldPosition = monster.transform.position;
        evolutionUI.transform.position = worldPosition + Vector3.up;
        evolutionUI.SetActive(true);
        SearchMonsterID();
    }

    public void Hide()
    {
        evolutionUI.SetActive(false);
    }

    // List -> Dict로 변환
    private void SetSprite()
    {
        if (evolutionSpritePairList == null) return;

        foreach (EvolutionSprite data in evolutionSpritePairList)
        {
            if (!evolutionSpriteDict.ContainsKey(data.monsterID))
            {
                evolutionSpriteDict.Add(data.monsterID, (data.evolutionType, data.sprite));
            }
        }
    }

    // 진화 이미지 설정
    public void SearchMonsterID()
    {
        foreach(int key in evolutionSpriteDict.Keys)
        {
            Debug.Log($"key:{key}, monsterID: {selectMonster.data.monsterId}");
            if(key == (int)selectMonster.data.monsterId)
            {
                SetEvolutionImage(key);       
            }
        }
    }

    private void SetEvolutionImage(int key)
    {
        Sprite[] sprites = evolutionSpriteDict[key].Item2;

        for (int i = 0; i < typeImages.Length; i++)
        {
            typeImages[i].sprite = sprites[i];
            Debug.Log($"sprite {sprites[i]}");
        }
    }
}
