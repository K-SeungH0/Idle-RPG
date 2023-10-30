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
		//LogManager.Instance.PrintLog(LogManager.enLogType.Normal, "PlayerStateAttack is On!!!", LogManager.LogColor.Blue);
		param._enCurrentState = ActorEnum.State.Attack;
		var animator = param.GetAnimator();
		animator.SetTrigger("OnAttack");
		//GameManager.Instance.cameraShake.SetIsCameraMoving(false);
		if(GameManager.Instance != null)
			GameManager.Instance.CameraMove.SwitchState(CameraMovement.enCameraState.Stop);
    }

    public override void LateUpdate()
	{
		base.LateUpdate();
    }

    public override void End()
	{
		animationHelper.OnStartAnimation -= OnStartAnimation;
		animationHelper.OnEndAnimation -= OnEndAnimation;
		//LogManager.Instance.PrintLog(LogManager.enLogType.Normal, "PlayerStateAttack is OVer!!!", LogManager.LogColor.Blue);
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
	Animator animator;
	public override void Start()
	{
		//LogManager.Instance.PrintLog(LogManager.enLogType.Normal, "PlayerStateWalk is On!!!", LogManager.LogColor.Blue);

		param._enCurrentState = ActorEnum.State.Walk;
        if (animator == null)
            animator = param.GetAnimator();
		animator.SetTrigger("OnWalk");

		//GameManager.Instance.cameraShake.SetIsCameraMoving(true);
		GameManager.Instance.CameraMove.SwitchState(CameraMovement.enCameraState.Move);

		//param.transform.DOMoveX(param.transform.position.x + 10f, 3f).OnComplete(()=>
		//{
		//	//param.SwitchState(ActorEnum.State.Back);
		//});

		//GameManager.Instance.cameraShake.CameraMove();
	}
    public override void UpdateState()
    {
        animator.SetTrigger("OnWalk");
        Vector3 newPosition = param.transform.position + 6.0f * Time.deltaTime * Vector3.right;
        param.transform.position = newPosition;
    }

	public override void End()
	{
		//LogManager.Instance.PrintLog(LogManager.enLogType.Normal, "PlayerStateWalk is Over!!!", LogManager.LogColor.Blue);
	}
}
public class PlayerStateBack : PlayerState
{
	public override void Start()
	{
		param._enCurrentState = ActorEnum.State.Back;
		var animator = param.GetAnimator();
		animator.SetTrigger("OnWalk");

		//param.transform.DOMoveX(param.transform.position.x - 1.5f, 0.5f).OnComplete(() =>
		//{
		//	param.SwitchState(ActorEnum.State.Idle);
		//});

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

public class PlayerStateAttackerWalk : PlayerState 
{
    Animator animator;
    public override void Start()
    {
        //LogManager.Instance.PrintLog(LogManager.enLogType.Normal, "PlayerStateWalk is On!!!", LogManager.LogColor.Blue);

        param._enCurrentState = ActorEnum.State.Walk;
        if (animator == null)
            animator = param.GetAnimator();
        animator.SetTrigger("OnWalk");
    }
    public override void UpdateState()
    {
        animator.SetTrigger("OnWalk");
        Vector3 newPosition = param.transform.position + Time.deltaTime * Vector3.left * param.GetPlayerMoveSpeed();
        param.transform.position = newPosition;
    }

    public override void End()
    {
        //LogManager.Instance.PrintLog(LogManager.enLogType.Normal, "PlayerStateWalk is Over!!!", LogManager.LogColor.Blue);
    }
}

