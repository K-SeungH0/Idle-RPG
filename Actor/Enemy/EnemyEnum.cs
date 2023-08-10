using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyEnum
{
	public enum EnemyState
	{
		None,
		Idle,
		Walk,
		Attack,
		Hit,
	}
	/// <summary> 에너미 공격 타입 </summary>
   public enum AttackType
	{
		None = 0,
		Melee,
		Range,
	}
}
