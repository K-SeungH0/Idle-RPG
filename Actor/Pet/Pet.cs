//using System.Collections;
//using System.Collections.Generic;
//using System.Numerics;
//using UnityEngine;

//public class Pet : MonoBehaviour, IActor
//{
//    private bool _isInit = false;
//	private bool _isInitState = false;

//	[Space]
//	[SerializeField]
//	private Animator _animator;
//	[SerializeField]
//	private AnimationHelper _animationHelper;

//	[SerializeField]
//	private float _fAttackRange;

//	[SerializeField]
//	private PoolEnums.PoolProjectileId _projectileId;

//	private IAttackMachineUsecase _atkMachineUsecase = new AttackMachineUsecase();

//	/// <summary> Pety FSM </summary>
//	private StateMachine<Pet> _petState;
//	public ActorEnum.State _enCurrentState { get; set; }

//	sActorStat _petStats = new sActorStat();
//	private int _level = 1;

//	public void Init(bool isMainUnit = false)
//	{
//		if (_isInit == false)
//		{
//			_isInit = true;
//		}
//	}
//	public void InitState()
//	{
//		if (!_isInitState)
//		{
//			_petState = new StateMachine<Pet>(this);

//			_petState.AddState(new PetStateIdle());
//			_petState.AddState(new PetStateWalk(transform.position));
//			_petState.AddState(new PetStateAttack());
//			_petState.AddState(new PetStateHit());
//			_petState.AddState(new PetStateDie());
//			_petState.AddState(new PetStateBack());

//			_isInitState = true;
//		}
//	}
//	public void SetPetData(PetData data)
//	{
//		_petStats.Atk = AppManager.Instance.GetDataTableManager().GetStatGroupDataTable().GetDataByLevel(data.stat_group1, _level).stat_value;
//		_petStats.AtkSpeed = AppManager.Instance.GetDataTableManager().GetStatGroupDataTable().GetDataByLevel(data.stat_group2, _level).stat_value;
//		_atkMachineUsecase.Init();
//		InitAttackMachine(0, 0, (1 / (float)_petStats.AtkSpeed));
//	}
//	public void TakeDamage(BigInteger damage, PoolEnums.PoolFloatingDamageId type)
//	{

//	}
//	public void Enable()
//	{
//		transform.gameObject.SetActive(true);
//	}
//    private void LateUpdate()
//	{
//		if (_petState != null)
//			_petState.LateUpdate();
//	}
//    /// <summary> 어택머신 초기 설정하는 함수 </summary>
//    public void InitAttackMachine(int slotIndex, float remainTime, float coolTime)
//	{
//		//_atkMachineUsecase.Init();

//		int random = Random.Range(0, 2);
//		_atkMachineUsecase.Add(slotIndex, remainTime, coolTime);
//		_atkMachineUsecase.SkillCallBack(OnSkillCallback);
//	}

//	void Update()
//	{
//        if (GameManager.Instance.GetEnemyList().Count != 0 && GameManager.Instance.GetEnemyTarget() != null)
//		{
//			_atkMachineUsecase.SetTarget(GameManager.Instance.GetEnemyTarget());
//			_atkMachineUsecase.Update();
//		}
//		else
//		{
//			_atkMachineUsecase.SetTarget(null);
//		}
//	}
//	private void OnSkillCallback(List<int> skillCall)
//	{
		
//		for (int i = 0; i < skillCall.Count; i++)
//		{
//			SwitchState(ActorEnum.State.Attack);
//			SkillStart(skillCall[i]);
//		}
//	}
//	private void SkillStart(int skillId)
//	{
//		if (skillId == 0)
//		{
//			var projectile = PoolProjectileExtension.GetPool(_projectileId);
//			if (projectile != null)
//			{
//				ProjectileStats projectileStat = new ProjectileStats();
//				projectileStat.ElapsedTime = 0f;
//				projectileStat.Target = GameManager.Instance.GetEnemyTarget();
//				projectileStat.Start = transform;
//				projectileStat.Speed = 4.0f;
//				projectileStat.Damage = _petStats.Atk;
//				projectileStat.DamageTargetType = ProjectileStats.enDamageTarget.Enemy;
//				projectile.gameObject.GetComponent<ParabolicProjectile>().SetProjectile(projectileStat);
		
//			}
//		}
//	}
//	public void Disable()
//	{
//		gameObject.SetActive(false);
//	}
//	public UnityEngine.Vector2 GetPosition()
//    {
//        return this.transform.position;
//    }

//    public Transform GetTransform()
//    {
//        return transform;
//    }


//    public void ReturnToPool()
//    {
//        Destroy(gameObject);
//    }

//    public void SetMoveSpeed(float speed)
//    {
//        throw new System.NotImplementedException();
//    }

//    public void SetPosition(UnityEngine.Vector2 position)
//    {
//        transform.position = position; 
//    }

//    public void SwitchState(ActorEnum.State state)
//    {
//		if (_isInitState == false)
//			return;

//		switch (state)
//		{
//			case ActorEnum.State.Idle:
//				_petState.ChangeState<PetStateIdle>();
//				break;
//			case ActorEnum.State.Walk:
//				_petState.ChangeState<PetStateWalk>();
//				break;
//			case ActorEnum.State.Hit:
//				_petState.ChangeState<PetStateHit>();
//				break;
//			case ActorEnum.State.Attack:
//				_petState.ChangeState<PetStateAttack>();
//				break;
//			case ActorEnum.State.Die:
//				_petState.ChangeState<PetStateDie>();
//				break;
//			case ActorEnum.State.Back:
//				_petState.ChangeState<PetStateBack>();
//				break;
//		}
//	}
//	public Animator GetAnimator()
//	{
//		return _animator;
//	}
//	public AnimationHelper GetAnimationHelper()
//	{
//		return _animationHelper;
//	}

//    public void NormalAttack()
//    {
//        throw new System.NotImplementedException();
//    }
//}
