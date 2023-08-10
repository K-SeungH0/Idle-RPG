using System.Numerics;
public class ActorStats
{
    /// <summary> 공격력 </summary>
    public BigInteger BAtk;
    /// <summary> 공격력 (%) </summary>
    public BigInteger BAtkPer;

    /// <summary> 공격 속도 </summary>
    public float fAtkSpd;
    /// <summary> 공격 속도 (%) </summary>
    public float fAtkSpdPer;

    /// <summary> 공격 사거리 </summary>
    public int nAtkRange;
    /// <summary> 공격 사거리 (%) </summary>
    public float nAtkRangePer;

    /// <summary> 체력 </summary>
    public BigInteger BHp;
    /// <summary> 체력 (%) </summary>
    public BigInteger BHpPer;

    /// <summary> 체력 회복 </summary>
    public BigInteger BHpRecovery;
    /// <summary> 체력 회복 (%) </summary>
    public BigInteger BHpRecoveryPer;

    /// <summary> 치명타 확률 (%) </summary>
    public float fCrit;
    /// <summary> 치명타 피해 (%) </summary>
    public BigInteger BCritDmg;

    /// <summary> 이동 속도 </summary>
    public int nMoveSpd;
    /// <summary> 이동 속도 (%) </summary>
    public float nMoveSpdPer;

    /// <summary> 회피율 (%) </summary>
    public float fDodge;

}
