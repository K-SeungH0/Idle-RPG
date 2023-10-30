using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class CastleHitNode : MonoBehaviour, IActor
{
    [SerializeField] private DefenceCastle _defenceCastle;
    #region 사용안함
    public void Disable()
    {
    }

    public void Enable()
    {
    }

    public AnimationHelper GetAnimationHelper()
    {
        return null;
    }

    public Animator GetAnimator()
    {
        return null;
    }

    public void InitState()
    {
    }

    public void NormalAttack()
    {
    }

    public void ReturnToPool()
    {
    }

    public void SetMoveSpeed(float speed)
    {
    }

    public void SetPosition(UnityEngine.Vector2 position)
    {
    }

    public void SwitchState(ActorEnum.State state)
    {
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public UnityEngine.Vector2 GetPosition()
    {
        return transform.position;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void Init(bool isMainUnit)
    {
    }
    #endregion

    public void TakeDamage(BigInteger damage, PoolEnums.PoolFloatingDamageId damageType = PoolEnums.PoolFloatingDamageId.NormalDmg)
    {
        _defenceCastle.TakeDamage(damage, damageType);
    }
}
