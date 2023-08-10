using System;
using System.Collections.Generic;
public interface IPetSystem 
{
    void Init();
    PetSystem.PetSystemData Equip(enPetSlot petSlot, int nID, int nLevel);
    PetSystem.PetSystemData? UnEquip(enPetSlot petSlot);
    PetSystem.PetSystemData? GetSlotPetData(enPetSlot petSlot);
    bool IsEquipped(enPetSlot petSlot);
}

public class PetSystem : IPetSystem
{
    public struct PetSystemData
    {
        public int nID;
        public int nLevel;
    }

    private Dictionary<enPetSlot, PetSystemData> _currentPetDic;
    // 더미 데이터
    private Dictionary<int, Dictionary<int, PetData>> AllPetData__TEST = new Dictionary<int, Dictionary<int, PetData>>();
    public void Init()
    {
        _currentPetDic = new Dictionary<enPetSlot, PetSystemData>();
    }
    public PetSystemData Equip(enPetSlot petSlot, int nID, int nLevel)
    {
        #region 더미 데이터 
        PetData petData = new PetData();
        if (AllPetData__TEST.ContainsKey(nID))
        {
            if (AllPetData__TEST[nID].ContainsKey(nLevel))
            {
                petData = AllPetData__TEST[nID][nLevel];
            }
        }
        #endregion

        PetSystemData data = new PetSystemData();
        data.nID = nID;
        data.nLevel = nLevel;

        _currentPetDic.Add(petSlot, data);
        return data;
    }
    public PetSystemData? UnEquip(enPetSlot petSlot)
    {
        if (_currentPetDic.ContainsKey(petSlot))
        {
            PetSystemData data = _currentPetDic[petSlot];
            _currentPetDic.Remove(petSlot);
            return data;
        }
        else
            return null;
    }
    public PetSystemData? GetSlotPetData(enPetSlot petSlot)
    {
        if (_currentPetDic.ContainsKey(petSlot))
            return _currentPetDic[petSlot];
        else 
            return null;
    }

    public bool IsEquipped(enPetSlot petSlot)
    {
        if (_currentPetDic.ContainsKey(petSlot))
            return true;
        else
            return false;
    }
}
