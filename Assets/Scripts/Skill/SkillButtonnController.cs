using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillButtonnController : MonoBehaviour
{
    private SkillSO skillSO;
    [SerializeField] private int skillIdx;

    // 현재 쿨타임이 걸려 있는가 true : 스킬 사용 불가, false : 스킬 사용 가능
    private bool isOnCollDown = false;
    private bool isClickSkillButton = false;
    [SerializeField] private GameObject skillRangeImage;
    [SerializeField] private RectTransform skillRangeImageRect;

    private Coroutine coroutine;
    private WaitForSeconds skillCoolDown;

    private void Awake()
    {
        skillSO = DataManager.Instance.GetSkillByIndex(skillIdx);
        skillCoolDown = new WaitForSeconds(skillSO.cooldown);
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
    }

    // 스킬 버튼을 클릭했을 때
    public void ClickSkillButton()
    {
        if (isOnCollDown) return;

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
        isOnCollDown = true;
        skillRangeImage.SetActive(false);
        Debug.Log("스킬 사용");
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(CoSkillCool());
    }

    private IEnumerator CoSkillCool()
    {
        yield return skillCoolDown;
        isOnCollDown = false;
    }
}
