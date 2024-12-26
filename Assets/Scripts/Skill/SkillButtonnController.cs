using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillButtonnController : MonoBehaviour
{
    private DataTable.Skill_Data _skillData; // 스킬 데이터

    // 현재 쿨타임이 걸려 있는가 true : 스킬 사용 불가, false : 스킬 사용 가능
    private bool _isOnCoolDown = false;
    private bool _isClickSkillButton = false; // 현재 스킬 사용하도록 스킬버튼을 눌렀는지 여부
    private GameObject _skillRangeSprite; // 스킬의 범위를 볼 수 있는 오브젝트
    [SerializeField] private GameObject _cancelBtn; // 취소 버튼

    private float _timeSinceSkill; // 스킬을 사용하고 난 후 경과 시간
    [SerializeField] private Image _blackImage; // 쿨타임 표시할 이미지 (360도로 fillamount함)
    [SerializeField] private Image _skillImage; // skillSprite가 들어갈 Image 컴포넌트

    [SerializeField] private PoolManager.PoolConfig _poolConfig;

    private void Awake()
    {
        int skillIdx = _poolConfig.prefab.GetComponent<Skill>().SkillIdx;
        _skillData = DataManager.Instance.GetSkillByIndex(skillIdx);
        _timeSinceSkill = 0f;
        PoolManager.Instance.AddPool(_poolConfig);
    }

    private void Start()
    {
        SetSkillImage();
    }

    private void Update()
    {
        if (_isClickSkillButton)
        {
            Vector3 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            newPos.z = 0f;
            _skillRangeSprite.transform.position = newPos;

            if (Input.GetMouseButtonDown(0) &&
                EventSystem.current != null && !EventSystem.current.IsPointerOverGameObject())
            {
                // skillImage는 Raycast Target 제외해 놔서 인식 안됨
                _isClickSkillButton = false;
                UseSkill();
            }
        }

        if (_isOnCoolDown)
        {
            _timeSinceSkill += Time.deltaTime;
            ChangeCooldownImage();
            
            if (_timeSinceSkill >= _skillData.cooldown)
            {
                _isOnCoolDown = false;
                _timeSinceSkill = 0f;
            }
        }
    }

    // 스킬 아이콘 이미지랑 스킬 범위 이미지 설정
    private void SetSkillImage()
    {
        _skillImage.sprite = _poolConfig.prefab.GetComponent<SpriteRenderer>().sprite;
        _skillRangeSprite = StageManager.Instance.skillRangeSprite;
    }

    // 스킬 버튼을 클릭했을 때
    public void ClickSkillButton()
    {
        if (_isOnCoolDown) return;

        _isClickSkillButton = !_isClickSkillButton;
        // 스킬 범위 보여지고 있으면 비활성화
        if (!_isClickSkillButton)
        {
            SetSkillRangeImage(false);
        }
        else ShowSkillRange();
    }

    private void SetSkillRangeImage(bool active)
    {
        _skillRangeSprite.SetActive(active);
        _cancelBtn.SetActive(active);
    }

    // 스킬 범위 보여줌
    private void ShowSkillRange()
    {
        _skillRangeSprite.transform.localScale = Vector2.one * _skillData.range;
        SetSkillRangeImage(true);
    }

    // 스킬 사용
    private void UseSkill()
    {
        _isOnCoolDown = true;
        SetSkillRangeImage(false);

        GameObject obj = PoolManager.Instance.SpawnFromPool(_skillData.skillName.ToString());
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
        float remainingTime = _skillData.cooldown - _timeSinceSkill;
        _blackImage.fillAmount = (remainingTime / _skillData.cooldown);
    }
}
