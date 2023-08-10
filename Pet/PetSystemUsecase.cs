using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPetSystemUsecase
{
    /// <summary>
    /// PetSystem 초기화
    /// </summary>
    /// <param name="statusSystemUsecase">Status System Usecase</param>
    void Init(IStatusSystemUsecase statusSystemUsecase);

    /// <summary>
    /// 펫 장착
    /// </summary>
    /// <param name="petSlot">장착할 슬롯</param>
    /// <param name="nID">펫의 ID</param>
    /// <param name="nLevel">펫의 Level</param>
    /// <returns>장착한 펫 Data</returns>
    PetSystem.PetSystemData Equip(enPetSlot petSlot, int nID, int nLevel);

    /// <summary>
    /// 펫 해제
    /// </summary>
    /// <param name="petSlot">해제할 슬롯</param>
    void UnEquip(enPetSlot petSlot);
}

public class PetSystemUsecase : IPetSystemUsecase
{
    IStatusSystemUsecase _IStatusSystemUsecase;
    IPetSystem _IPetSystem;

    public PetSystemUsecase()
    {
        _IPetSystem = new PetSystem();
    }
    public void Init(IStatusSystemUsecase statusSystemUsecase)
    {
        _IStatusSystemUsecase = statusSystemUsecase;
        _IPetSystem.Init();
    }
    public PetSystem.PetSystemData Equip(enPetSlot petSlot, int nID, int nLevel)
    {
        if (_IPetSystem.IsEquipped(petSlot))
            UnEquip(petSlot);

        var equipPetData = _IPetSystem.Equip(petSlot, nID, nLevel);
        _IStatusSystemUsecase.RegisterStatus(enPlayerType.Player, enStatusType.Pet, nID, nLevel);

        return equipPetData;

    }
    public void UnEquip(enPetSlot petSlot)
    {
        if (_IPetSystem.IsEquipped(petSlot))
        {
            var unEquipPetData = _IPetSystem.UnEquip(petSlot); 

            if(unEquipPetData != null)
                _IStatusSystemUsecase.UnRegisterStatus(enPlayerType.Player, enStatusType.Pet, unEquipPetData.Value.nID, unEquipPetData.Value.nLevel);
        }
    }
}