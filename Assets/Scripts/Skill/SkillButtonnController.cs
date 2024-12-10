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
    [SerializeField] private int skillIdx; // 스킬 인덱스

    // 현재 쿨타임이 걸려 있는가 true : 스킬 사용 불가, false : 스킬 사용 가능
    private bool isOnCoolDown = false;
    private bool isClickSkillButton = false; // 현재 스킬 사용하도록 스킬버튼을 눌렀는지 여부
    private Button parentButton;
    [SerializeField] private GameObject skillRangeImage; // 스킬의 범위를 볼 수 있는 오브젝트
    [SerializeField] private RectTransform skillRangeImageRect; // 스킬 범위를 설정할 RectTransform
    [SerializeField] private GameObject cancelBtn; // 취소 버튼

    private Coroutine coroutine; // 스킬 쿨타임 코루틴
    private WaitForSeconds skillCoolDown; // 스킬 쿨타임
    private float timeSinceSkill; // 스킬을 사용하고 난 후 경과 시간
    [SerializeField] private Image blackImage; // 쿨타임 표시할 이미지 (360도로 fillamount함)

    private Dictionary<SkillName, Sprite> skillSpriteDict = new Dictionary<SkillName, Sprite>(); // 스킬 스프라이트 담을 Dict
    
    [System.Serializable]
    private class SkillSpritePair
    {
        public SkillName skillName;
        public Sprite sprite;
    }
    [SerializeField] private List<SkillSpritePair> skillSpritePairList; // Inspector에서 Sprite 넣기 위해 사용하는 List
    [SerializeField] private Image skillImage; // skillSprite가 들어갈 Image 컴포넌트

    private void Awake()
    {
        skillSO = DataManager.Instance.GetSkillByIndex(skillIdx);
        skillCoolDown = new WaitForSeconds(skillSO.cooldown); // 스킬 쿨타임과 동기화
        timeSinceSkill = 0f;
        parentButton = GetComponent<Button>();
        SetSprite();
        SetSkillImage();
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
                Debug.Log("Use Skill");
                UseSkill();
            }
        }

        if (isOnCoolDown)
        {
            timeSinceSkill += Time.deltaTime;
            ChangeCooldownText();
            
            if (timeSinceSkill >= skillSO.cooldown)
            {
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
            if(!skillSpriteDict.ContainsKey(pair.skillName))
            {
                skillSpriteDict.Add(pair.skillName, pair.sprite);
            }
        }
    }

    // 스킬 아이콘 이미지랑 스킬 범위 이미지 설정
    private void SetSkillImage()
    {
        skillImage.sprite = skillSpriteDict[skillSO.skillName];
        skillRangeImageRect.sizeDelta = new Vector2(skillSO.range, skillSO.range); // 범위 설정
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
        Debug.Log("스킬 사용");
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(CoSkillCool());
    }

    // 스킬 사용 후 쿨타임 계산 코루틴
    private IEnumerator CoSkillCool()
    {
        yield return skillCoolDown;
        isOnCoolDown = false;
        blackImage.fillAmount = 0f;
    }

    // 쿨타임 Text 변경
    private void ChangeCooldownText()
    {
        float remainingTime = skillSO.cooldown - timeSinceSkill;
        blackImage.fillAmount = (remainingTime / skillSO.cooldown);
    }
}
