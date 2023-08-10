using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 보유 하고 있는 Item 관리
/// </summary>
public class ItemDataManager : Singleton<ItemDataManager>
{
    public struct ItemData 
    {
        /// <summary> 보유한 아이템 레벨 </summary>
        public int nLevel;
        /// <summary> 보유한 아이템 갯수 </summary>
        public int nCount;
    }

    /// <summary> 현재 보유 하고 있는 장비 </summary>
    private Dictionary<enEquipType, Dictionary<int, ItemData>> _haveEquipData = new Dictionary<enEquipType, Dictionary<int, ItemData>>();

    public void InitItem(Dictionary<enEquipType, Dictionary<int, ItemData>> items)
    {
        _haveEquipData = items;
    }

    public ItemData AddItem(enEquipType type, int id)
    {
        if (_haveEquipData.ContainsKey(type))
        {
            ItemData weaponData = new ItemData();

            if (_haveEquipData[type].ContainsKey(id))
            {
                // 기존에 있던 장비
                weaponData.nLevel = _haveEquipData[type][id].nLevel;
                weaponData.nCount = _haveEquipData[type][id].nCount + 1;
                _haveEquipData[type][id] = weaponData;
            }
            else
            {
                // 처음 얻었다..!
                weaponData.nLevel = 1;
                weaponData.nCount = 1;
                _haveEquipData[type].Add(id, weaponData);
            }
            return weaponData;
        }
        else
        {
            // 여기는 들어오면 안되지만.. 혹시나 해서..
            _haveEquipData.Add(type, new Dictionary<int, ItemData>());
            return AddItem(type, id);
        }
    }
    public ItemData? GetItem(enEquipType type, int id)
    {
        if (_haveEquipData.ContainsKey(type))
        {
            if (_haveEquipData[type].ContainsKey(id))
                return _haveEquipData[type][id];
            else 
                return null;
        }
        else
        {
            return null;
        }

    }

    #region Getter & Setter
    public Dictionary<enEquipType, Dictionary<int, ItemData>> GetHaveItems()
    {
        return _haveEquipData;
    }
    #endregion
}