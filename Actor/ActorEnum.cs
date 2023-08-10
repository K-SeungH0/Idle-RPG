using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorEnum
{
    public enum State
    {
        None = 0,
        Idle = 1,
        Walk = 2,
        Hit = 3,
        Attack = 4,
        Die = 5,
        Back = 6,
    }

    public enum EnemyAttackType
    {
        None = 0,
        Melee,      // 근거리 타입
        Range,      // 원거리 타입
    }

    public enum enPlayerStats
    {
        /// <summary> 공격력 </summary>
        Atk,
        /// <summary> 체력 </summary>
        Hp,
        /// <summary> 체력 회복 </summary>
        HpRecovery,
        /// <summary> 공격 속도 </summary>
        AtkSpd,
        /// <summary> 치명타 확률 (%) </summary>
        Crit,
        /// <summary> 치명타 피해 (%) </summary>
        CritDmg,
        /// <summary> 더블샷 </summary>
        DoubleShot,
        /// <summary> 트리플샷 </summary>
        TripleShot,

        MAX
    }
}
