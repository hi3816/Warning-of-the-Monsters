using UnityEngine;

public class SkillButtonController : MonoBehaviour
{
    [SerializeField] int _maxSkillCount;
    [SerializeField] private PoolManager.PoolConfig[] _poolConfigArr;
    [SerializeField] private GameObject _skillRangeSprite; // 스킬의 범위를 볼 수 있는 오브젝트
    
    public GameObject[] SkillRangeSpriteArr { get; private set; }
    public Sprite[] SkillSpriteArr { get; private set; } 
    public int[] SkillIdxArr { get; private set; }

    private void Awake()
    {
        SkillRangeSpriteArr = new GameObject[_maxSkillCount];
        SkillSpriteArr = new Sprite[_maxSkillCount];
        SkillIdxArr = new int[_maxSkillCount];

        for (int i = 0; i < _maxSkillCount; i++)
        {
            SkillRangeSpriteArr[i] = Instantiate(_skillRangeSprite);
            SkillRangeSpriteArr[i].SetActive(false);
            SkillSpriteArr[i] = _poolConfigArr[i].prefab.GetComponent<SpriteRenderer>().sprite;
            SkillIdxArr[i] = _poolConfigArr[i].prefab.GetComponent<Skill>().SkillIdx;
        }
        PoolManager.Instance.AddPools(_poolConfigArr);
    }

    public bool CheckOtherSkillClick(int buttonIdx)
    {
        for (int i = 0; i < SkillRangeSpriteArr.Length; i++)
        {
            if (buttonIdx != i && SkillRangeSpriteArr[i].gameObject.activeSelf) return true;  
        }
        return false;
    }
}
