using DG.Tweening;
using UnityEngine;
public class PlayerState : IState<Player>
{
    protected string state;

    protected PlayerState() { }

    public PlayerState(string state)
    {
        this.state = state;
    }
}

public class PlayerStateIdle : PlayerState
{
	
	public override void Start()
	{
		//LogManager.Instance.PrintLog(LogManager.enLogType.Normal, "PlayerIdle is On!!!", LogManager.LogColor.Blue);
	
		param._enCurrentState = ActorEnum.State.Idle;
		var animator = param.GetAnimator();
		animator.SetTrigger("OnIdle");
	}


	public override void LateUpdate()
	{

	}

	public override void End()
	{
		//LogManager.Instance.PrintLog(LogManager.enLogType.Normal, "PlayerIdle is Over!!!", LogManager.LogColor.Blue);
		
	}

	
}


public class PlayerStateAttack : PlayerState
{
	AnimationHelper animationHelper;
	public override void Start()
	{
		param._enCurrentState = ActorEnum.State.Attack;
		animationHelper = param.GetAnimationHelper();
		animationHelper.OnStartAnimation += OnStartAnimation;
		animationHelper.OnEndAnimation += OnEndAnimation;
		LogManager.Instance.PrintLog(LogManager.enLogType.Normal, "PlayerStateAttack is On!!!", LogManager.LogColor.Blue);
		param._enCurrentState = ActorEnum.State.Attack;
		var animator = param.GetAnimator();
		animator.SetTrigger("OnAttack");

	}

	public override void LateUpdate()
	{
		base.LateUpdate();
	}

	public override void End()
	{
		animationHelper.OnStartAnimation -= OnStartAnimation;
		animationHelper.OnEndAnimation -= OnEndAnimation;
		LogManager.Instance.PrintLog(LogManager.enLogType.Normal, "PlayerStateAttack is OVer!!!", LogManager.LogColor.Blue);
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

public class PlayerStateWalk : PlayerState
{
	public override void Start()
	{
		LogManager.Instance.PrintLog(LogManager.enLogType.Normal, "PlayerStateWalk is On!!!", LogManager.LogColor.Blue);

		param._enCurrentState = ActorEnum.State.Walk;
		var animator = param.GetAnimator();
		animator.SetTrigger("OnWalk");
		
		
		param.transform.DOMoveX(param.transform.position.x + 1.5f, 3f).OnComplete(()=>
		{
			param.SwitchState(ActorEnum.State.Back);
		});

		//param.transform.DOMoveX(param.transform.position.x - 1.5f, 2f).OnComplete(() =>
		//{
		//	param.SwitchState(ActorEnum.State.Idle);
		//});

	}

	public override void LateUpdate()
	{
		
	}

	public override void End()
	{
		LogManager.Instance.PrintLog(LogManager.enLogType.Normal, "PlayerStateWalk is Over!!!", LogManager.LogColor.Blue);
	}
}
public class PlayerStateBack : PlayerState
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
		//LogManager.Instance.PrintLog(LogManager.enLogType.Normal, "PlayerStateWalk is Over!!!", LogManager.LogColor.Blue);
	}
}
public class PlayerStateDie : PlayerState
{
	public override void Start()
	{
		//LogManager.Instance.PrintLog(LogManager.enLogType.Normal, "PlayerStateDie is Over!!!", LogManager.LogColor.Blue);

		param._enCurrentState = ActorEnum.State.Die;
		var animator = param.GetAnimator();
		animator.SetTrigger("OnDie");
	}

	public override void End()
	{
		//LogManager.Instance.PrintLog(LogManager.enLogType.Normal, "PlayerStateDie is On!!!", LogManager.LogColor.Blue);
	}
}