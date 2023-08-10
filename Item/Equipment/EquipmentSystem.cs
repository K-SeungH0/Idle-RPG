using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
public interface IEquipmentSystem
{
    void Init();
    EquipmentSystem.EquipmentInfo Equip(int nID, int nLevel);
    EquipmentSystem.EquipmentInfo? UnEquip(int nID, int nLevel);
    EquipmentSystem.EquipmentInfo? GetEquipment(enEquipType equipSlot);
    bool IsEquipped(enEquipType equipSlot);

    Dictionary<enEquipType, EquipmentSystem.EquipmentInfo> GetCurrentEquipment();
}
public class EquipmentSystem : IEquipmentSystem
{
    public struct EquipmentInfo
    {
        public int nID;
        public int nLevel;
    }
    private Dictionary<enEquipType, EquipmentInfo> _currentEquipmentDic;
    public void Init()
    {
        _currentEquipmentDic = new Dictionary<enEquipType, EquipmentInfo>();
    }

    public EquipmentInfo Equip(int nID, int nLevel)
    {
        var equipment = AppManager.Instance.GetDataTableManager().GetEquipmentDataTable().GetData(nID);
        EquipmentInfo data = new EquipmentInfo();
        data.nID = nID;
        data.nLevel = nLevel;
        _currentEquipmentDic.Add(equipment.Equip_type, data);
        return data;
    }

    public EquipmentInfo? UnEquip(int nID, int nLevel)
    {
        var equipment = AppManager.Instance.GetDataTableManager().GetEquipmentDataTable().GetData(nID);

        if (_currentEquipmentDic.ContainsKey(equipment.Equip_type))
        {
            EquipmentInfo data = _currentEquipmentDic[equipment.Equip_type];
            _currentEquipmentDic.Remove(equipment.Equip_type);
            return data;
        }
        else
            return null;
    }


    public EquipmentInfo? GetEquipment(enEquipType equipSlot)
    {
        if (_currentEquipmentDic.ContainsKey(equipSlot))
            return _currentEquipmentDic[equipSlot];
        else
            return null;
    }

    public bool IsEquipped(enEquipType equipSlot)
    {
        if (_currentEquipmentDic.ContainsKey(equipSlot))
            return true;
        else 
            return false;
    }

    public Dictionary<enEquipType, EquipmentInfo> GetCurrentEquipment()
    {
        return _currentEquipmentDic;
    }
}