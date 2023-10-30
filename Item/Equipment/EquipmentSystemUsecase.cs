using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public interface IEquipmentSystemUsecase
{
    /// <summary>
    /// 임시용 초기화
    /// </summary>
    void Init();
    /// <summary>
    /// EquipmentSystem 초기화
    /// </summary>
    /// <param name="statusSystemUsecase">Status System Usecase</param>
    void Init(IStatusSystemUsecase statusSystemUsecase);

    /// <summary>
    /// 장비 장착
    /// </summary>
    /// <param name="nID">장비의 ID</param>
    /// <param name="nLevel">장비의 Level</param>
    /// <returns>장착한 장비 Data</returns>
    EquipmentSystem.EquipmentInfo Equip(int nID, int nLevel);

    /// <summary>
    /// 장비 해제
    /// </summary>
    /// <param name="nID">장비의 ID</param>
    /// <param name="nLevel">장비의 Level</param>
    void UnEquip(int nID, int nLevel);

    /// <summary>
    /// 해당 슬롯에 장착한 장비를 얻어 오는 함수
    /// </summary>
    /// <param name="slot">필요한 장비</param>
    /// <returns>해당 슬롯 장비의 ID, Level</returns>
    EquipmentSystem.EquipmentInfo? GetEquipment(enEquipType slot);
    /// <summary>
    /// 현재 장착중인 모든 장비를 얻어 오는 함수
    /// </summary>
    /// <returns>장착중인 장비</returns>
    Dictionary<enEquipType, EquipmentSystem.EquipmentInfo> GetCurrentEquipment();
}

public class EquipmentSystemUsecase : IEquipmentSystemUsecase
{
    /// <summary>
    /// 임시용 초기화 인가?
    /// </summary>
    bool isTemp = false;
    IEquipmentSystem _IEquipmentSystem;
    IStatusSystemUsecase _IStatusSystemUsecase;

    public EquipmentSystemUsecase()
    {
        _IEquipmentSystem = new EquipmentSystem();
    }
    public void Init()
    {
        _IEquipmentSystem.Init();
        isTemp = true;
    }
    public void Init(IStatusSystemUsecase statusSystemUsecase)
    {
        _IStatusSystemUsecase = statusSystemUsecase;
        _IEquipmentSystem.Init();
    }
    public EquipmentSystem.EquipmentInfo Equip(int nID, int nLevel)
    {
        var equipment = AppManager.Instance.GetDataTableManager().GetEquipmentDataTable().GetData(nID);
        if (_IEquipmentSystem.IsEquipped(equipment.Equip_type)) // 해당 장비 슬롯이 장착중이면
            UnEquip(nID, nLevel);

        var equipData = _IEquipmentSystem.Equip(nID, nLevel);

        if(isTemp == false)
            _IStatusSystemUsecase.RegisterStatus(enPlayerType.Player, enStatusType.Equipment, equipData.nID, equipData.nLevel);
        else
        {
            var equipItem = ItemDataManager.Instance.GetItem(equipment.Equip_type, equipment.id);
            if (equipItem != null)
            {
                var stat = AppManager.Instance.GetDataTableManager().GetStatGroupDataTable().GetDataByLevel(equipment.stat_group, equipItem.Value.nLevel);

                if(stat.stat_type == 1)
                    GameManager.Instance.GetPlayer().GetPlayerStats().BAtk += stat.stat_value;
                else if(stat.stat_type == 9)
                    GameManager.Instance.GetPlayer().GetPlayerStats().BHp += stat.stat_value;
            }
            else
                LogManager.Instance.PrintLog(LogManager.enLogType.Error, "보유 하고 있지 않은 장비를 장착함!!!");
        }
        return equipData;
    }

    public void UnEquip(int nID, int nLevel)
    {
        var equipment = AppManager.Instance.GetDataTableManager().GetEquipmentDataTable().GetData(nID);
        if (_IEquipmentSystem.IsEquipped(equipment.Equip_type)) // 해당 장비 슬롯이 장착중이면
        {
            var unEquipData = _IEquipmentSystem.UnEquip(nID, nLevel);

            if (unEquipData != null)
            {
                if (isTemp == false)
                    _IStatusSystemUsecase.UnRegisterStatus(enPlayerType.Player, enStatusType.Equipment, nID, nLevel); 
                else
                {
                    var equipItem = ItemDataManager.Instance.GetItem(equipment.Equip_type, equipment.id);
                    if (equipItem != null)
                    {
                        var stat = AppManager.Instance.GetDataTableManager().GetStatGroupDataTable().GetDataByLevel(equipment.stat_group, equipItem.Value.nLevel);

                        if (stat.stat_type == 1)
                            GameManager.Instance.GetPlayer().GetPlayerStats().BAtk -= stat.stat_value;
                        else if (stat.stat_type == 9)
                            GameManager.Instance.GetPlayer().GetPlayerStats().BHp -= stat.stat_value;
                    }
                    else
                        LogManager.Instance.PrintLog(LogManager.enLogType.Error, "보유 하고 있지 않은 장비를 해제함!!!");
                }
            }
        }
    }

    public EquipmentSystem.EquipmentInfo? GetEquipment(enEquipType slot)
    {
        var equipmentData = _IEquipmentSystem.GetEquipment(slot);

        if (equipmentData != null)
            return equipmentData.Value;
        else 
            return null;
    }

    public Dictionary<enEquipType, EquipmentSystem.EquipmentInfo> GetCurrentEquipment()
    {
        return _IEquipmentSystem.GetCurrentEquipment();
    }
}
