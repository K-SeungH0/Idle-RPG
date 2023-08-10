using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface ISkillSystemUsecase
{
    /// <summary>
    /// SkillSystem 초기화
    /// </summary>
    /// <param name="statusSystemUsecase">Status System Usecase</param>
    void Init(IStatusSystemUsecase statusSystemUsecase);

    /// <summary>
    /// 스킬 장착
    /// </summary>
    /// <param name="skillSlot">장착할 슬롯</param>
    /// <param name="nID">스킬의 ID</param>
    /// <param name="nLevel">스킬의 Level</param>
    /// <returns>장착한 스킬 Data</returns>
    Skill Equip(enSkillSlot skillSlot, int nID, int nLevel);

    /// <summary>
    /// 스킬 해제
    /// </summary>
    /// <param name="skillSlot">해제할 슬롯</param>
    void UnEquip(enSkillSlot skillSlot);
    /// <summary>
    /// 장착중인 스킬 정보
    /// </summary>
    /// <returns>Slot을 Key값으로하는 Dictionary</returns>
    Dictionary<enSkillSlot, Skill> GetCurrentEquipSkills();
}
public class SkillSystemUsecase : ISkillSystemUsecase
{
    ISkillSystem _IskillSystem;
    IStatusSystemUsecase _IStatusSystemUsecase;

    public SkillSystemUsecase()
    {
        _IskillSystem = new SkillSystem();
    }
    public void Init(IStatusSystemUsecase statusSystemUsecase)
    {
        _IStatusSystemUsecase = statusSystemUsecase;
        _IskillSystem.Init();
    }
    public Skill Equip(enSkillSlot skillSlot, int nID, int nLevel)
    {
        if (_IskillSystem.IsEquipped(skillSlot)) 
            UnEquip(skillSlot);

        var equipSkill = _IskillSystem.Equip(skillSlot, nID, nLevel);
        // TODO _IStatusSystemUsecase에다가 equipSkill 전달
        _IStatusSystemUsecase.RegisterStatus(enPlayerType.Player, enStatusType.Skill, nID, nLevel);
        return equipSkill;
    }
    public void UnEquip(enSkillSlot skillSlot)
    {
        if (_IskillSystem.IsEquipped(skillSlot))
        {
            var unEquipSkill = _IskillSystem.UnEquip(skillSlot);

            if (unEquipSkill != null)
                _IStatusSystemUsecase.UnRegisterStatus(enPlayerType.Player, enStatusType.Skill, unEquipSkill.Value.ID, unEquipSkill.Value.level);
        }
    }

    public Dictionary<enSkillSlot, Skill> GetCurrentEquipSkills()
    {
        return _IskillSystem.GetCurrentEquipSkills();
    }
}
