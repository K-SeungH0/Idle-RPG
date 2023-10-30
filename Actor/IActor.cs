using System.Numerics;
using UnityEngine;

public interface IActor
{
	void Init(bool isMainUnit);
	void InitState();
	void Enable();
	void SetPosition(UnityEngine.Vector2 position);
	void SetMoveSpeed(float speed);
	/// <summary> 상태 변경하는 함수 </summary>
	/// <param name="state"></param>
	void SwitchState(ActorEnum.State state);
	/// <summary> 데미지에 따른 HP 변화 함수(현재는 Int형, 추후 BigInteger로 변경예정) </summary>
	/// <param name="damage"></param>
	void TakeDamage(BigInteger damage, PoolEnums.PoolFloatingDamageId damageType = PoolEnums.PoolFloatingDamageId.NormalDmg);
	UnityEngine.Vector2 GetPosition();
	Transform GetTransform();
	GameObject GetGameObject();
	void ReturnToPool();
	Animator GetAnimator();
	AnimationHelper GetAnimationHelper();
	void Disable();
	void NormalAttack();

	Transform transform { get { return GetTransform(); } }

	GameObject gameObject { get { return GetGameObject(); } }
}

public struct sActorStat
{
	public BigInteger Atk;
	public BigInteger Hp;
	public int AtkSpeed;
	public int MoveSpeed;
}
