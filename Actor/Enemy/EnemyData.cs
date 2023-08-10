using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData : ScriptableObject
{
    [SerializeField]
    public EnemyEnum.AttackType attackType;
    [SerializeField]
    public int level;
    [SerializeField]
    public int hp;

}
