using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkillSystem 
{
    void Init();
    Skill Equip(enSkillSlot skillSlot, int nID, int nLevel);
    Skill? UnEquip(enSkillSlot skillSlot);
    Skill? GetSlotSkillData(enSkillSlot skillSlot);
    bool IsEquipped(enSkillSlot skillSlot);
    Dictionary<enSkillSlot, Skill> GetCurrentEquipSkills();
}

public class SkillSystem : ISkillSystem
{
    private Dictionary<enSkillSlot, Skill> _currentEquipSkillDic;
    private Dictionary<int, Dictionary<int, Skill>> AllSkillDataTable__TEST = new Dictionary<int, Dictionary<int, Skill>>();

    public void Init()
    {
        _currentEquipSkillDic = new Dictionary<enSkillSlot, Skill>();
    }
    public Skill Equip(enSkillSlot skillSlot, int nID, int nLevel)
    {
        #region 더미 데이터
        Skill skill = new Skill();
        if(AllSkillDataTable__TEST.ContainsKey(nID))
        {
            if (AllSkillDataTable__TEST[nID].ContainsKey(nLevel))
            {
                skill = AllSkillDataTable__TEST[nID][nLevel];
            }
        }
        #endregion

        _currentEquipSkillDic.Add(skillSlot, skill);
        return skill;
    }
    public Skill? UnEquip(enSkillSlot skillSlot)
    {
        Skill? data;
        if (_currentEquipSkillDic.ContainsKey(skillSlot))
        {
            data = _currentEquipSkillDic[skillSlot];
            _currentEquipSkillDic.Remove(skillSlot);
        }
        else
            data = null;

        return data;
    }

    public Skill? GetSlotSkillData(enSkillSlot skillSlot)
    {
        if (_currentEquipSkillDic.ContainsKey(skillSlot))
            return _currentEquipSkillDic[skillSlot];
        else
            return null;
    }
    public bool IsEquipped(enSkillSlot skillSlot)
    {
        if (_currentEquipSkillDic.ContainsKey(skillSlot))
            return true;
        else 
            return false;
    }

    public Dictionary<enSkillSlot, Skill> GetCurrentEquipSkills()
    {
        return _currentEquipSkillDic;
    }
}
