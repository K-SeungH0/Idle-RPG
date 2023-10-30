using System.Numerics;

[System.Serializable]
public class PlayerStats : ActorStats
{
	/// <summary> 아이디 </summary>
	public int id;
	/// <summary> 레벨 </summary>
	public int Level;
    
	/// <summary> 더블 샷 </summary>
	public float fDoubleShot;
	/// <summary> 트리플 샷 </summary>
    public float fTripleShot;

	/// <summary> 골드 획득량 증가</summary>
	public BigInteger BGoldGain;
	/// <summary> 방치 보상 증가 </summary>
	public BigInteger BRewardGain;

	/// <summary> 현재 골드 </summary>
	public BigInteger BGold;
    /// <summary> 현재 보석 </summary>
    public BigInteger BGem;
	/// <summary> 경험치 </summary>
	public BigInteger BExp;

	// 임시 데이터
	public int Atk;
	public int HP;
	public int HPRecovery;

	/// <summary> 스탯 강화로 올라가는 공격력 </summary>
	public BigInteger BStatsAtk;
	/// <summary> 스탯 강화로 올라가는 공격속도 </summary>
	public float fStatsAtkSpd;
	/// <summary> 스탯 강화로 올라가는 더블샷 </summary>
    public float fStatsDoubleShot;
	/// <summary> 스탯 강화로 올라가는 트리플샷 </summary>
    public float fStatsTripleShot;
	/// <summary> 스탯 강화로 올라가는 체력 </summary>
    public BigInteger BStatsHp;
	/// <summary> 스탯 강화로 올라가는 체력회복 </summary>
    public BigInteger BStatsHpRecovery;
	/// <summary> 스탯 강화로 올라가는 치명타 확률 </summary>
    public float fStatsCritRate;
	/// <summary> 스탯 강화로 올라가는 치명타 피해 </summary>
    public BigInteger BStatsCritDmg;

	public BigInteger GetDamage()
	{
		BigInteger result = BAtk + BStatsAtk;
		return result;
	}
	public BigInteger GetCritDamage()
	{
        BigInteger result = BAtk + BStatsAtk;
        if ((BCritDmg + BStatsCritDmg) / 100 < 10000)
        {
            var dmg = (float)(BCritDmg + BStatsCritDmg) / 100f;
            var plusDmg = (float)result * dmg;
            result = (int)plusDmg;
        }
        else
            result = result * (BCritDmg + BStatsCritDmg) / 100;

		UnityEngine.Debug.Log("치명타 성공!! Damage : " + result.ToString());
        return result;
    }
    public bool IsCrit()
	{
		return Util.CommonUtil.GetPercentageChance(fCrit);
    }
}
