using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetState : IState<Pet>
{
	protected string state;

	protected PetState() { }

	public PetState(string state)
	{
		this.state = state;
	}

	protected AnimationHelper animationHelper;
}

public class PetStateIdle : PetState
{
	public override void Start()
	{
		animationHelper = param.GetAnimationHelper();
		//LogManager.Instance.PrintLog(LogManager.enLogType.Normal, "PetIdle is On!!!", LogManager.LogColor.Blue);

		param._enCurrentState = ActorEnum.State.Idle;

		var animator = param.GetAnimator();
		animator.SetTrigger("OnIdle");
	}


	public override void LateUpdate()
	{

	}

	public override void End()
	{
		//LogManager.Instance.PrintLog(LogManager.enLogType.Normal, "PetIdle is Over!!!", LogManager.LogColor.Blue);
	}
}

public class PetStateWalk : PetState
{

	public override void Start()
	{
		//LogManager.Instance.PrintLog(LogManager.enLogType.Normal, "PetIdle is On!!!", LogManager.LogColor.Blue);

		param._enCurrentState = ActorEnum.State.Walk;

		var animator = param.GetAnimator();
		animator.SetTrigger("OnWalk");

		param.transform.DOMoveX(param.transform.position.x + 1.5f, 3f).OnComplete(() =>
		{
			param.SwitchState(ActorEnum.State.Back);
			//param.transform.DOMoveX(param.transform.position.x - 1.5f, 2f).OnComplete(() =>
			//{
			//	param.SwitchState(ActorEnum.State.Idle);
			//});
		});
	}


	public override void LateUpdate()
	{

	}

	public override void End()
	{
		//LogManager.Instance.PrintLog(LogManager.enLogType.Normal, "PetIdle is Over!!!", LogManager.LogColor.Blue);
	}
}
public class PetStateBack : PetState
{
	public override void Start()
	{
		
		param._enCurrentState = ActorEnum.State.Back;

		param.transform.DOMoveX(param.transform.position.x - 1.5f, 2f).OnComplete(() =>
		{
			param.SwitchState(ActorEnum.State.Idle);
		});
	}


	public override void LateUpdate()
	{

	}

	public override void End()
	{

	}
}

public class PetStateAttack : PetState
{
	public override void Start()
	{
		animationHelper = param.GetAnimationHelper();
		animationHelper.OnEndAnimation += OnEndAnimation;
		//LogManager.Instance.PrintLog(LogManager.enLogType.Normal, "PetIdle is On!!!", LogManager.LogColor.Blue);

		param._enCurrentState = ActorEnum.State.Attack;

		var animator = param.GetAnimator();
		animator.SetTrigger("OnAttack");
	}


	public override void LateUpdate()
	{

	}

	public override void End()
	{
		//LogManager.Instance.PrintLog(LogManager.enLogType.Normal, "PetIdle is Over!!!", LogManager.LogColor.Blue);
	}
	private void OnStartAnimation()
	{
		//LogManager.Instance.PrintLog(LogManager.enLogType.Normal, "StartAnim???", LogManager.LogColor.Pink);
	}
	private void OnEndAnimation()
	{
		param.SwitchState(ActorEnum.State.Idle);

	}
}

public class PetStateHit : PetState
{
	public override void Start()
	{
		//LogManager.Instance.PrintLog(LogManager.enLogType.Normal, "PetIdle is On!!!", LogManager.LogColor.Blue);

		param._enCurrentState = ActorEnum.State.Hit;

		var animator = param.GetAnimator();
		animator.SetTrigger("OnHit");
	}


	public override void LateUpdate()
	{

	}

	public override void End()
	{
		//LogManager.Instance.PrintLog(LogManager.enLogType.Normal, "PetIdle is Over!!!", LogManager.LogColor.Blue);
	}
}

public class PetStateDie : PetState
{
	public override void Start()
	{
		//LogManager.Instance.PrintLog(LogManager.enLogType.Normal, "PetIdle is On!!!", LogManager.LogColor.Blue);

		param._enCurrentState = ActorEnum.State.Die;

		var animator = param.GetAnimator();
		animator.SetTrigger("OnDie");
	}


	public override void LateUpdate()
	{

	}

	public override void End()
	{
		//LogManager.Instance.PrintLog(LogManager.enLogType.Normal, "PetIdle is Over!!!", LogManager.LogColor.Blue);
	}
}
