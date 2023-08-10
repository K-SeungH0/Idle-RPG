using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum enState
{
	Idle = 0,
	Walk,
	Attack,
	Hit,
	Die,
	Max,
}

public class EnemyState : IState<Enemy>
{
	protected string state;
    protected EnemyState() { }
    public EnemyState(string state)
    {
        this.state = state;
    }
}
public class EnemyStateIdle : EnemyState
{
	public override void Start()
	{
		//LogManager.Instance.PrintLog(LogManager.enLogType.Normal, "EnemyIdleStart", LogManager.LogColor.Yellow);
		param._enCurrentState = ActorEnum.State.Idle;
		var animator = param.GetAnimator();
		animator.SetTrigger("OnIdle");

	}
	public override void LateUpdate()
	{

	}

	public override void End()
	{
		//LogManager.Instance.PrintLog(LogManager.enLogType.Normal, "EnemyIdleEnd", LogManager.LogColor.Red);
	}
}
public class EnemyStateWalk : EnemyState
{
	AnimationHelper animationHelper;
	private Vector3 _vPosition;
	private Player _player;

	public override void Start()
	{
		//LogManager.Instance.PrintLog(LogManager.enLogType.Normal, "EnemyWalkStart", LogManager.LogColor.Yellow);
		
		var animator = param.GetAnimator();
		animator.SetTrigger("OnWalk");
		_vPosition = param.GetPosition();
		_player = param.GetTargetPlayer();
	}

	public override void LateUpdate()
	{
		//if (param.transform.position.x <= -1)
		//{
		//	param.SwitchState(ActorEnum.State.Attack);
		//	return;
		//}

		if(Vector3.Distance(_player.transform.position, param.transform.position) < param.GetEnemyRange() || param.transform.position.x <= -1)
		{
			param.SwitchState(ActorEnum.State.Attack);
			return;
		}
			

		//if (param.transform.position.x - _player.transform.position.x <= 1.5f)
		//{
		//	param.SwitchState(ActorEnum.State.Idle);
		//	return;
		//}

		

		float moveSpeed = param.GetMoveSpeed();

		// 왼쪽 방향으로 이동
		Vector3 direction = Vector3.left;
		_vPosition += direction * moveSpeed * Time.deltaTime;

		// 이동한 위치 설정
		param.SetPosition(_vPosition);

		// 플레이어와 충돌하면 공격 상태로 전환 또는 BoxCollider(MeleeAttackZone)에 닿으면 공격상태로 전환
		
	}


	public override void End()
	{
		//LogManager.Instance.PrintLog(LogManager.enLogType.Normal, "EnemyWalkEnd", LogManager.LogColor.Red);
	}

}

public class EnemyStateAttack : EnemyState
{
	AnimationHelper animationHelper;

	public override void Start()
	{
		animationHelper = param.GetAnimationHelper();
		animationHelper.OnStartAnimation += OnStartAnimation;
		animationHelper.OnEndAnimation += OnEndAnimation;
		animationHelper.OnStartFireAnimation += OnFireAnimation;
		//LogManager.Instance.PrintLog(LogManager.enLogType.Normal, "EnemyAttackStart", LogManager.LogColor.Yellow);
		var animator = param.GetAnimator();
		animator.SetTrigger("OnAttack");
	}
	public override void LateUpdate()
	{

	}

	public override void End()
	{
		
	}

	private void OnStartAnimation()
	{
		
	}
	private void OnEndAnimation()
	{
		param.SwitchState(ActorEnum.State.Idle);

		GameManager.Instance.GetPlayer().PlayAnimation("Hit", null);
	}
	private void OnFireAnimation()
	{
	}
}
public class EnemyStateDie : EnemyState
{
	public override void Start()
	{
		param.EnemyDieEffect();
		var animator = param.GetAnimator();
		animator.SetTrigger("OnDie");

		var animationHelper = param.GetAnimationHelper();
		animationHelper.OnEndAnimation += OnEndAnimation;
		
		GameManager.Instance.GetPlayer().GainGold(GameManager.Instance.GetEnemyGoldForCurrentStage());

        param.ReturnToPool();
	}
	public override void LateUpdate()
	{

	}

	public override void End()
	{

	}
	private void OnEndAnimation()
	{
		
	}
}
