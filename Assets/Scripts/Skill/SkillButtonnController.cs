using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillButtonnController : MonoBehaviour
{
    private SkillSO skillSO; // 스킬 데이터
    [SerializeField] public int SkillIdx { get; private set; } // 스킬 인덱스

    // 현재 쿨타임이 걸려 있는가 true : 스킬 사용 불가, false : 스킬 사용 가능
    private bool isOnCoolDown = false;
    private bool isClickSkillButton = false; // 현재 스킬 사용하도록 스킬버튼을 눌렀는지 여부
    [SerializeField] private GameObject skillRangeImage; // 스킬의 범위를 볼 수 있는 오브젝트
    [SerializeField] private RectTransform skillRangeImageRect; // 스킬 범위를 설정할 RectTransform
    [SerializeField] private GameObject cancelBtn; // 취소 버튼

    private float timeSinceSkill; // 스킬을 사용하고 난 후 경과 시간
    [SerializeField] private Image blackImage; // 쿨타임 표시할 이미지 (360도로 fillamount함)

    private Dictionary<int, Sprite> skillSpriteDict = new Dictionary<int, Sprite>(); // 스킬 스프라이트 담을 Dict
    
    [System.Serializable]
    private class SkillSpritePair
    {
        public int id;
        public Sprite sprite;
    }
    [SerializeField] private List<SkillSpritePair> skillSpritePairList; // Inspector에서 Sprite 넣기 위해 사용하는 List
    [SerializeField] private Image skillImage; // skillSprite가 들어갈 Image 컴포넌트

    [SerializeField] private PoolManager.PoolConfig[] poolConfigs;

    private void Awake()
    {
        skillSO = DataManager.Instance.GetSkillByIndex(SkillIdx);
        timeSinceSkill = 0f;
        SetSprite();
        SetSkillImage();
        PoolManager.Instance.AddPoolS(poolConfigs);
    }

    private void Update()
    {
        if (isClickSkillButton)
        {
            skillRangeImage.transform.position = Input.mousePosition;

            if (Input.GetMouseButtonDown(0) &&
                EventSystem.current != null && !EventSystem.current.IsPointerOverGameObject())
            {
                // skillImage는 Raycast Target 제외해 놔서 인식 안됨
                isClickSkillButton = false;
                UseSkill();
            }
        }

        if (isOnCoolDown)
        {
            timeSinceSkill += Time.deltaTime;
            ChangeCooldownImage();
            
            if (timeSinceSkill >= skillSO.cooldown)
            {
                isOnCoolDown = false;
                timeSinceSkill = 0f;
            }
        }
    }

    // List -> Dict로 변환
    private void SetSprite()
    {
        if (skillSpritePairList == null) return;

        foreach (SkillSpritePair pair in skillSpritePairList)
        {
            if(!skillSpriteDict.ContainsKey(pair.id))
            {
                skillSpriteDict.Add(pair.id, pair.sprite);
            }
        }
    }

    // 스킬 아이콘 이미지랑 스킬 범위 이미지 설정
    private void SetSkillImage()
    {
        skillImage.sprite = skillSpriteDict[skillSO.id];
        skillRangeImageRect.sizeDelta = new Vector2(skillSO.range * 0.5f, skillSO.range * 0.5f); // 범위 설정
    }

    // 스킬 버튼을 클릭했을 때
    public void ClickSkillButton()
    {
        if (isOnCoolDown) return;

        isClickSkillButton = !isClickSkillButton;
        // 스킬 범위 보여지고 있으면 비활성화
        if (!isClickSkillButton)
        {
            skillRangeImage.SetActive(false);
            cancelBtn.SetActive(false);
        }
        else ShowSkillRange();
    }

    // 스킬 범위 보여줌
    private void ShowSkillRange()
    {
        skillRangeImage.SetActive(true);
        cancelBtn.SetActive(true);
    }

    // 스킬 사용
    private void UseSkill()
    {
        isOnCoolDown = true;
        skillRangeImage.SetActive(false);
        cancelBtn.SetActive(false);

        GameObject obj = PoolManager.Instance.SpawnFromPool(skillSO.skillName.ToString());
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPosition.z = 0f; // Z축 값 고정
        obj.transform.position = worldPosition;
        Skill skill = obj.GetComponent<Skill>();
        skill.StartSkill();
        obj.SetActive(true);
    }

    // 쿨타임 이미지 변경
    private void ChangeCooldownImage()
    {
        float remainingTime = skillSO.cooldown - timeSinceSkill;
        blackImage.fillAmount = (remainingTime / skillSO.cooldown);
    }
}
