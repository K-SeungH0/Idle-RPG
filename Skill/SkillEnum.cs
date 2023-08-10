public enum enSkillSlot
{
    None = 0,
    Slot1,
    Slot2,
    Slot3,
    Slot4,
    Slot5,
    Slot6,
}
public enum enSkillGrade 
{
    Common = 0,     // 커먼
    UnCommon,       // 언커먼
    Rare,           // 레어
    Epic,           // 에픽
    Legendary,      // 레전더리
    Mythic,         // 신화
    Transcendence,  // 초월
}

/// <summary>
/// None, NormalAttack, Active, Passive, Have
/// </summary>
public enum enSkillType
{ 
    None = 0,
    /// <summary> 슬라임/펫 일반 공격 </summary>
    NormalAttack,
    /// <summary> 계정 스킬, 슬라임 스킬 중 엑티브 스킬 </summary>
    Active,
    /// <summary> 계정 스킬, 슬라임 스킬 중 패시브 스킬 </summary>
    Passive,
    /// <summary> 아이템, 펫, 유물 등의 보유 효과를 지정 </summary>
    Have
}

public struct Skill
{
    public int ID;              // 스킬 ID
    public string name;         // 스킬 이름
    public string description;  // 스킬 설명
    public int level;           // 스킬 레벨
    public string icon;         // 스킬 아이콘
    public enSkillGrade grade;  // 스킬 등급
    public bool isEquip;        // 장착 중 인지?
}
