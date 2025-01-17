using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillButtonn : MonoBehaviour
{
    [Header("Util")]
    private DataTable.Skill_Data _skillData; // 스킬 데이터
    private SkillButtonController _skillButtonController;
    private bool _isOnCoolDown = false; // 쿨타임 유무
    private float _timeSinceSkill; // 스킬을 사용하고 난 후 경과 시간

    [Header("Button")]
    [SerializeField] private int _skillButtonIdx;
    [SerializeField] private GameObject _cancelBtn; // 취소 버튼
    private Button _skillButton;
    private bool _isClickSkillButton = false; // 스킬버튼을 눌렀는지 여부

    [Header("Image")]
    [SerializeField] private Image _blackImage; // 쿨타임 표시할 이미지 (360도로 fillamount함)
    [SerializeField] private Image _skillImage; // skillSprite가 들어갈 Image 컴포넌트
    [SerializeField] private GameObject _skillRangeSprite;

    private void Awake()
    {
        _skillButton = GetComponent<Button>();
        _skillButton.onClick.AddListener(() => ClickSkillButton());
        _skillButtonController = GetComponentInParent<SkillButtonController>();
        _timeSinceSkill = 0f;
    }

    private void Start()
    {
        _skillData = DataManager.Instance.GetSkillByIndex(_skillButtonController.SkillIdxArr[_skillButtonIdx]);
        _skillImage.sprite = _skillButtonController.SkillSpriteArr[_skillButtonIdx];
        _skillRangeSprite = _skillButtonController.SkillRangeSpriteArr[_skillButtonIdx];
        _skillRangeSprite.transform.localScale = Vector2.one * _skillData.range;
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

    // 스킬 버튼을 클릭했을 때
    private void ClickSkillButton()
    {
        if (_skillButtonController.CheckOtherSkillClick(_skillButtonIdx)) return;
        if (_isOnCoolDown) return;
        _isClickSkillButton = !_isClickSkillButton;

        if (!_isClickSkillButton) SetSkillRangeImage(false);
        else SetSkillRangeImage(true);
    }

    private void SetSkillRangeImage(bool active)
    {
        _skillRangeSprite.SetActive(active);
        _cancelBtn.SetActive(active);
    }

    // 스킬 사용
    private void UseSkill()
    {
        _isOnCoolDown = true;
        SetSkillRangeImage(false);

        Skill skill = PoolManager.Instance.SpawnFromPool<Skill>(_skillData.skillName.ToString());
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPosition.z = 0f; // Z축 값 고정
        skill.transform.position = worldPosition;
        skill.StartSkill();
        skill.gameObject.SetActive(true);
    }

    // 쿨타임 이미지 변경
    private void ChangeCooldownImage()
    {
        float remainingTime = _skillData.cooldown - _timeSinceSkill;
        _blackImage.fillAmount = (remainingTime / _skillData.cooldown);
    }
}
