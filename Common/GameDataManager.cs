using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class GameDataManager : Singleton<GameDataManager>
{
    private List<DropShipData> _attackerDropShipData = new List<DropShipData>();
    private bool _isAttacker = false;

    #region Property
    public List<DropShipData> AttackersDropShipData 
    {
        get { return _attackerDropShipData; } 
        set 
        {
            _attackerDropShipData = null;
            _attackerDropShipData = value.ToList();
        }
    }
    public bool IsAttacker 
    {
        get { return _isAttacker; } 
        set { _isAttacker = value; }
    }
    #endregion

}
