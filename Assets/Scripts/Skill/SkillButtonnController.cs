using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillButtonnController : MonoBehaviour
{
    private SkillSO skillSO; // 스킬 데이터
    [SerializeField] private int skillIdx; // 스킬 인덱스

    // 현재 쿨타임이 걸려 있는가 true : 스킬 사용 불가, false : 스킬 사용 가능
    private bool isOnCoolDown = false;
    private bool isClickSkillButton = false; // 현재 스킬 사용하도록 스킬버튼을 눌렀는지 여부
    [SerializeField] private GameObject skillRangeImage; // 스킬의 범위를 볼 수 있는 오브젝트
    [SerializeField] private RectTransform skillRangeImageRect; // 스킬 범위를 설정할 RectTransform

    private Coroutine coroutine; // 스킬 쿨타임 코루틴
    private WaitForSeconds skillCoolDown; // 스킬 쿨타임
    private float remainingCooldown; // 남아있는 쿨타임
    [SerializeField] private TextMeshProUGUI coolDownText; // 남은 쿨타임을 보여줄 Text

    private void Awake()
    {
        skillSO = DataManager.Instance.GetSkillByIndex(skillIdx);
        skillCoolDown = new WaitForSeconds(skillSO.cooldown); // 스킬 쿨타임과 동기화
        remainingCooldown = 0f;
    }

    private void Update()
    {
        if (isClickSkillButton)
        {
            skillRangeImage.transform.position = Input.mousePosition;

            if (Input.GetMouseButtonDown(0))
            {
                isClickSkillButton = false;
                UseSkill();
            }
        }

        if (isOnCoolDown)
        {
            remainingCooldown += Time.deltaTime;
            ChangeCooldownText();
            
            if (remainingCooldown >= skillSO.cooldown)
            {
                remainingCooldown = 0f;
                coolDownText.enabled = false;
            }
        }
    }

    // 스킬 버튼을 클릭했을 때
    public void ClickSkillButton()
    {
        if (isOnCoolDown) return;

        ShowSkillRange();
    }

    // 스킬 범위 보여줌
    private void ShowSkillRange()
    {
        isClickSkillButton = true;
        skillRangeImage.SetActive(true);
        skillRangeImageRect.sizeDelta = new Vector2(skillSO.range, skillSO.range); // 범위 설정
    }

    private void UseSkill()
    {
        isOnCoolDown = true;
        skillRangeImage.SetActive(false);
        coolDownText.enabled = true;
        Debug.Log("스킬 사용");
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(CoSkillCool());
    }

    private IEnumerator CoSkillCool()
    {
        yield return skillCoolDown;
        isOnCoolDown = false;
    }

    private void ChangeCooldownText()
    {
        coolDownText.text = (skillSO.cooldown - remainingCooldown).ToString("F2");
    }
}
