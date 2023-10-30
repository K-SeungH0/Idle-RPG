using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;
using Newtonsoft.Json;

public class SnapShot
{
    public const string LocalSnapDataKey = "Local_SnapShot_Data";

    [Serializable]
    public struct SnapShotData
    {
        /// <summary> 골드 누적 (before) </summary>
        public BigInteger GoldBefore;
        /// <summary> 골드 누적 (after) </summary>
        public BigInteger GoldAfter;
        /// <summary> 골드 변화량 (value) </summary>
        public BigInteger GoldChange;

        /// <summary> 보석 DB </summary>
        public GemData GemData;

        /// <summary> 레벨 </summary>
        public int Level;
        /// <summary> 경험치 </summary>
        public BigInteger Exp;
        /// <summary> 광고 타입 </summary>
        public AdvertiseType AdvertiseType;

        /// <summary> 플레이어 스탯 </summary>
        public PlayerStats PlayerStats;
        /// <summary> 장비 레벨 </summary>
        public Dictionary<enEquipType, Dictionary<int, ItemDataManager.ItemData>> EquipmentLevels;
        /// <summary> 현재 장착중인 장비들 </summary>
        public Dictionary<enEquipType, EquipmentSystem.EquipmentInfo> CurrentEquipments;
        /// <summary> 현재 스테이지 ID </summary>
        public int StageID;
        /// <summary> 드랍십 정보 </summary>
        public DropShipInfo DropShipInfo;
    }
    [Serializable]
    public struct GemData
    {
        /// <summary> 보석 누적 (before) </summary>
        public BigInteger GemBefore;
        /// <summary> 보석 누적 (after) </summary>
        public BigInteger GemAfter;
        /// <summary> 보석 변화량 (value) </summary>
        public BigInteger GemChange;
    }
    public struct SnapShotLocalData
    {
        /// <summary> Local 현재 골드 </summary>
        public BigInteger Gold;
        /// <summary> Local 현재 보석 </summary>
        public BigInteger Gem;
        /// <summary> Local 현재 레벨 </summary>
        public int Level;
        /// <summary> Local 현재 경험치 </summary>
        public BigInteger Exp;
        /// <summary> 서버 시간 </summary>
        public DateTime ServerTime;

        /// <summary> 플레이어 스탯 레벨 </summary>
        public Dictionary<enStatusElement, int> PlayerStatsLevel;
        //public PlayerStats PlayerStats;
        /// <summary> 장비 레벨 </summary>
        public Dictionary<enEquipType, Dictionary<int, ItemDataManager.ItemData>> EquipmentLevels;
        /// <summary> 현재 장착중인 장비들 </summary>
        public Dictionary<enEquipType, EquipmentSystem.EquipmentInfo> CurrentEquipments;
        /// <summary> 현재 스테이지 ID </summary>
        public int StageID;
        /// <summary> 드랍십 정보 </summary>
        public DropShipInfo DropShipInfo;
    }

    public struct GemDB
    {
        /// <summary> 게임 데이터 키 </summary>
        public string key;
        public GemData gemData;
    }

    private static SnapShotData _serverSnapData;
    private static SnapShotLocalData _localSnapData;

    public static SnapShotData ServerSnapData
    {
        get { return _serverSnapData; }
        //set { _serverSnapData = value; } 
    }
    public static SnapShotLocalData LocalSnapData
    {
        get { return _localSnapData; }
        //set { _localSnapData = value; }
    }
    public static async UniTaskVoid SaveAsync(System.Threading.CancellationToken token)
    {
        while (true)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(60), cancellationToken: token);
            //SaveSnapShot();
            SaveLocal();
        }
    }

    #region Server SnapData
    public static void SaveSnapShot()
    {
        LogManager.Instance.PrintLog(LogManager.enLogType.Normal, "☆★☆★☆★ SaveSnapShot Success ☆★☆★☆★", LogManager.LogColor.Cyan);

        _serverSnapData.GoldBefore = _serverSnapData.GoldAfter;
        _serverSnapData.GoldAfter = _localSnapData.Gold;
        _serverSnapData.GoldChange = _localSnapData.Gold - _serverSnapData.GoldAfter;

        _serverSnapData.GemData.GemBefore = _serverSnapData.GemData.GemAfter;
        _serverSnapData.GemData.GemAfter = _localSnapData.Gem;
        _serverSnapData.GemData.GemChange = _localSnapData.Gem - _serverSnapData.GemData.GemAfter;

        _serverSnapData.Level = _localSnapData.Level;
        _serverSnapData.Exp = _localSnapData.Exp;

        // TODO 광고 타입 추가
        _serverSnapData.AdvertiseType = AdvertiseType.None;

        // 여기서 서버에 SnapData를 저장한다.
        //_serverSnapData
    }
    public static void LoadSnapShot()
    {
        //LogManager.Instance.PrintLog(LogManager.enLogType.Normal, "☆★☆★☆★ LoacSnapShot Success ☆★☆★☆★", LogManager.LogColor.Cyan);

        // 여기서 서버 SnapData를 받아온다.
        var serverdata = new SnapShotData();

        _serverSnapData = serverdata;
    }
    #endregion


    #region Local SnapData
    // Json string을 암호화, 복호화 하고 object로 변환한다.
    // 암호화, 복호화 순서
    // object -> Json 변환 -> 암호화 -> 복호화 -> Json 변환 -> object
    public static void SaveLocal()
    {
        LogManager.Instance.PrintLog(LogManager.enLogType.Normal, "☆★☆★☆★ SaveLocal Success ☆★☆★☆★", LogManager.LogColor.Cyan);

        // TODO Player 데이터로 변경 해야함.
        var playerStats = GameManager.Instance.GetPlayer().GetPlayerStats();
        _localSnapData.EquipmentLevels = ItemDataManager.Instance.GetHaveItems();
        _localSnapData.CurrentEquipments = GameManager.Instance.GetPlayer().GetEquipmentSystemUsecase().GetCurrentEquipment();

        _localSnapData.Gold = playerStats.BGold;
        //_localSnapData.Gem = BigInteger.Parse("9012384091241818412481414842948091898");
        _localSnapData.Level = playerStats.Level;
        _localSnapData.Exp = playerStats.BExp;
        
        _localSnapData.ServerTime = DateTime.Now; // TOOD 서버 시간으로 변경
        _localSnapData.StageID = GameManager.Instance.GetCurrentStageID();
        _localSnapData.PlayerStatsLevel = GameManager.Instance.GetAbilityManager().GetPlayerStatsLevel();

        AESEncrypt ase = new AESEncrypt();
        string snapDataJson = JsonConvert.SerializeObject(_localSnapData);
        var data = ase.AESEncrypt256(snapDataJson);
        PlayerPrefs.SetString(LocalSnapDataKey, Convert.ToBase64String(data));
        PlayerPrefs.Save();
    }
    public static void LoadLocal()
    {
        LogManager.Instance.PrintLog(LogManager.enLogType.Normal, "☆★☆★☆★ LoadLocal Success ☆★☆★☆★", LogManager.LogColor.Cyan);
        string localData = PlayerPrefs.GetString(LocalSnapDataKey);
        _localSnapData = new SnapShotLocalData();

        if (localData != string.Empty)
        {
            AESEncrypt aes = new AESEncrypt();
            var localDataJson = aes.AESDecrypt256(Convert.FromBase64String(localData));
            _localSnapData = JsonConvert.DeserializeObject<SnapShotLocalData>(localDataJson);
        }

        if (_localSnapData.CurrentEquipments == null)
        {
            // 해당 계정이 처음 시작했을때 여기로 들어온다.
            _localSnapData.CurrentEquipments = new Dictionary<enEquipType, EquipmentSystem.EquipmentInfo>();
        }
        if (_localSnapData.EquipmentLevels == null)
        {
            // 해당 계정이 처음 시작했을때 여기로 들어온다.
            var defalutWeapon = new Dictionary<int, ItemDataManager.ItemData>
            {
                { 10001, new ItemDataManager.ItemData() { nLevel = 1, nCount = 1 } },
                { 10002, new ItemDataManager.ItemData() { nLevel = 1, nCount = 1 } },
                { 10003, new ItemDataManager.ItemData() { nLevel = 1, nCount = 1 } },
                { 10004, new ItemDataManager.ItemData() { nLevel = 1, nCount = 1 } },
                { 10005, new ItemDataManager.ItemData() { nLevel = 1, nCount = 1 } },
                { 10006, new ItemDataManager.ItemData() { nLevel = 1, nCount = 1 } },
                { 10007, new ItemDataManager.ItemData() { nLevel = 1, nCount = 1 } }
            };
            var defalutArmor = new Dictionary<int, ItemDataManager.ItemData>
            {
                { 20001, new ItemDataManager.ItemData() { nLevel = 1, nCount = 1 } },
                { 20002, new ItemDataManager.ItemData() { nLevel = 1, nCount = 1 } },
                { 20003, new ItemDataManager.ItemData() { nLevel = 1, nCount = 1 } },
                { 20004, new ItemDataManager.ItemData() { nLevel = 1, nCount = 1 } },
                { 20005, new ItemDataManager.ItemData() { nLevel = 1, nCount = 1 } },
                { 20006, new ItemDataManager.ItemData() { nLevel = 1, nCount = 1 } },
                { 20007, new ItemDataManager.ItemData() { nLevel = 1, nCount = 1 } }
            };

            _localSnapData.EquipmentLevels = new Dictionary<enEquipType, Dictionary<int, ItemDataManager.ItemData>>
            {
                { enEquipType.Weapon, defalutWeapon },
                { enEquipType.Armor, defalutArmor },
            };
        }

        // TODO 시간 경과 SnapShot과 LocalSnapShot Merge
        // DateTime.Now -> 현재 서버 시간으로 변경 해야함.
        var ElapsedTime = DateTime.Now - _localSnapData.ServerTime;

        //GameManager.Instance.GetPlayer().SetPlayerStats(_localSnapData.PlayerStats);

        ItemDataManager.Instance.InitItem(_localSnapData.EquipmentLevels);

        foreach(var Equip in _localSnapData.CurrentEquipments)
        {
            GameManager.Instance.GetPlayer().GetEquipmentSystemUsecase().Equip(Equip.Value.nID, Equip.Value.nLevel);
        }

        if (_localSnapData.StageID != 0)
            GameManager.Instance.SetCurrentStageID(_localSnapData.StageID);
        else
            GameManager.Instance.SetCurrentStageID(10001); // 게임 처음 시작.

        GameManager.Instance.GetPlayer().GainGold(_localSnapData.Gold);
        GameManager.Instance.GetPlayer().GainGem(_localSnapData.Gem);

        if(_localSnapData.PlayerStatsLevel != null)
            GameManager.Instance.GetAbilityManager().SetPlayerStatsLevel(_localSnapData.PlayerStatsLevel, true);
    }
    #endregion

    public static void ApplyLocalData()
    {
        // TODO Player Stats Set
        // 보유 아이템 Load
        foreach (var equipType in _localSnapData.EquipmentLevels.Keys)
        {
            foreach (var id in _localSnapData.EquipmentLevels[equipType].Keys)
            {
                ItemDataManager.Instance.AddItem(equipType, id);
            }
        }

        // 장착중인 아이템 Load
        foreach (var equipInfo in _localSnapData.CurrentEquipments)
        {
            GameManager.Instance.GetPlayer().GetEquipmentSystemUsecase().Equip(equipInfo.Value.nID, equipInfo.Value.nLevel);
        }
    }
}
