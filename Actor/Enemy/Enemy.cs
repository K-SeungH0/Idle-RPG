using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public abstract class Enemy : MonoBehaviour, IActor
{
	[Header("HPBar")]
	[SerializeField]
	protected HpView _hpView;
	[Header("Animation")]
	[SerializeField]
	private Animator _animator;


	[Header("Target VFX Node")]
	///<summary> 발사체 나가는 노드 </summary>
	[SerializeField]
	protected Transform _trAttackNode;
	/// <summary> 타격 맞는 노드 </summary>
	[SerializeField]
	protected Transform _trHitNode;

	[SerializeField]
	protected AnimationHelper _animationHelper;

	[SerializeField]
	private Animation animation_source;

	private Coroutine Co_HandleAnimation;

	/// <summary> Enemy FSM </summary>
	protected StateMachine<Enemy> _enemyState;
	public ActorEnum.State _enCurrentState { get; set; }

	// =============================== Enemy 기본 데이터 들어가는 곳 ===============================
	protected EnemyEnum.AttackType _enAttackType;
	protected BigInteger _bMaxHp;
	protected BigInteger _bCurrentHp;
	protected int _level = 1;
	[SerializeField]
	protected int _fRange = 8;

	// =============================== Enemy Animation 관련 ===============================
	protected bool _isInit;
	protected bool _isInitState;
	private new Transform transform;

	protected EnemyData _enemyData;
	// =============================== Enemy To Player 관련 ===============================
	protected string _strPlayerTag;
	protected Player _player;

	protected float _fMoveSpeed;

	private float _fEndpointPosX;

	//  =============================== AttackMachine 관련 ===============================
	protected IAttackMachineUsecase _atkMachineUsecase = new AttackMachineUsecase();

	//  =============================== Enemy Data 관련 ===============================
	StatGroupDataTable _statgroupDataTable = new StatGroupDataTable();
	SkillInfoDataTable _skillInfoDataTable = new SkillInfoDataTable();
	SkillEffectDataTable _skillEffectDataTable = new SkillEffectDataTable();

	sActorStat _monsterStat = new sActorStat();

	MonsterData _monsterData = new MonsterData();

	#region InterFaceRegion

	public virtual void Init()
	{
		if (_isInit == false)
		{
			transform = this.gameObject.transform;
			_isInit = true;
			_fEndpointPosX = GameManager.Instance.GetEnemyEndPoint().position.x;
			
		}
	}
	public void InitState()
	{
		if (!_isInitState)
		{
			_enemyState = new StateMachine<Enemy>(this);

			_enemyState.AddState(new EnemyStateIdle());
			_enemyState.AddState(new EnemyStateAttack());
			_enemyState.AddState(new EnemyStateDie());
			_enemyState.AddState(new EnemyStateWalk());

			_isInitState = true;
		}
	}

	/// <summary> 스텟 그룹 데이터 테이블 </summary>
	private void InitStatGroupDataTable()
	{
		_statgroupDataTable = AppManager.Instance.GetDataTableManager().GetStatGroupDataTable();
		_skillInfoDataTable = AppManager.Instance.GetDataTableManager().GetSkillInfoDataTable();
		_skillEffectDataTable = AppManager.Instance.GetDataTableManager().GetSkillEffectDataTable();
	}
	public void InitAttackMachine(int slotIndex, float remainTime, float coolTime)
	{
		_atkMachineUsecase.Init();
		_atkMachineUsecase.Add(slotIndex, remainTime, coolTime);
		_atkMachineUsecase.SkillCallBack(OnSkillCallback);
	}
	/// <summary> Stat 1 : Atk Stat2 : AtkSpeed Stat3 : Hp Stat4 : MoveSpeed </summary>
	/// <param name="data"></param>
	public void SetEnemyData(MonsterData data)
	{
		InitStatGroupDataTable();
		_monsterData = data;
		//var list1 =  _statgroupDataTableDic[data.stat_group1];
		var statRate = GameManager.Instance.GetEnemyStatRate();

		float realRate = statRate / (float)Util.CommonUtil.STAT_PERCENT;

		_monsterStat.Atk = (BigInteger)(_statgroupDataTable.GetDataByLevel(data.stat_group1, _level).stat_value * realRate);
		_monsterStat.Hp = (BigInteger)(_statgroupDataTable.GetDataByLevel(data.stat_group2, _level).stat_value * realRate);
		_monsterStat.AtkSpeed = _statgroupDataTable.GetDataByLevel(data.stat_group3, _level).stat_value;
		_monsterStat.MoveSpeed = _statgroupDataTable.GetDataByLevel(data.stat_group4, _level).stat_value;

		int first = _skillInfoDataTable.GetData(data.atk_skill, 1).skillEffectID1;

		_fRange = _skillEffectDataTable.GetData(first).att_range;

		InitAttackMachine(0, 0, (1 / (float)_monsterStat.AtkSpeed));
		SetMoveSpeed(_monsterStat.MoveSpeed);
	}
	public void Enable()
	{
		_bMaxHp = _monsterStat.Hp;
		_bCurrentHp = _monsterStat.Hp;
		InitHpBar();
		transform.gameObject.SetActive(true);

		if(_isInitState == true)
			_enemyState.ChangeState<EnemyStateWalk>();
	}
	public void PlayAnimation(string animation_name, System.Action OnCompleted, bool is_check_end_animation = false)
	{
		if (!this.gameObject.activeInHierarchy)
			return; 

		if (!ReferenceEquals(Co_HandleAnimation, null))
			StopCoroutine(Co_HandleAnimation);

		Co_HandleAnimation = StartCoroutine(_PlayAnimation(animation_name, OnCompleted, is_check_end_animation));
	}
	

	private IEnumerator _PlayAnimation(string animation_name, System.Action OnCompleted, bool is_check_end_animation = false)
	{

		animation_source.Play(animation_name);

		if (is_check_end_animation)
		{
			yield return new WaitForSeconds(animation_source.GetClip(animation_name).length);
		}

		OnCompleted?.Invoke();
	}
	private void OnSkillCallback(List<int> skillCall)
	{
		for (int i = 0; i < skillCall.Count; i++)
		{
			SkillStart(skillCall[i]);
		}
	}
	private void SkillStart(int skillId)
	{
		//LogManager.Instance.PrintLog(LogManager.enLogType.Normal, "Skill ID : " + skillId, LogManager.LogColor.Blue);
		if(skillId != 0)
		{
			SwitchState(ActorEnum.State.Attack);


			var data = _skillInfoDataTable.GetData(_monsterData.atk_skill, _level);
			if(_skillEffectDataTable.GetData(data.skillEffectID1).att_range > 0)
			{
				//원거리 공격(Projectile)
				var projectile = PoolProjectileExtension.GetPool(PoolEnums.PoolProjectileId.Bullet_01_01);
				if (projectile != null)
				{
					ProjectileStats projectileStat = new ProjectileStats();
					projectileStat.ElapsedTime = 0f;
					projectileStat.Target = GameManager.Instance.GetPlayerTransform();
					projectileStat.Start = _trAttackNode;
					projectileStat.Speed = 4.0f;
					projectileStat.Damage = 5;
					projectileStat.DamageTargetType = ProjectileStats.enDamageTarget.Player;
					projectile.gameObject.GetComponent<ParabolicProjectile>().SetProjectile(projectileStat);
				}
			}
			else
			{
				//즉발공격
				_player.TakeDamage(_monsterStat.Atk);
			}
		}
		else
		{
			//원거리 공격(Projectile)
			var projectile = PoolProjectileExtension.GetPool(PoolEnums.PoolProjectileId.Bullet_01_01);
			if (projectile != null)
			{
				ProjectileStats projectileStat = new ProjectileStats();
				projectileStat.ElapsedTime = 0f;
				projectileStat.Target = GameManager.Instance.GetPlayerTransform();
				projectileStat.Start = _trAttackNode;
				projectileStat.Speed = 4.0f;
				projectileStat.Damage = 5;
				projectileStat.DamageTargetType = ProjectileStats.enDamageTarget.Player;
				projectile.gameObject.GetComponent<ParabolicProjectile>().SetProjectile(projectileStat);
			}
		}

	}
	public void SetTargetPlayer(Player player)
	{
		_player = player;
	}
	/// <summary> Actor들이 Target을 찾을 때 사용하는 함수 </summary>
	public void FindTarget(Transform target, float range)
	{
		if (target == null)
			return;
		else
		{
			if(this.transform != null)
			{
				float distance = transform.position.x - target.position.x;
				distance = Mathf.Abs(distance);

				if (distance < range || transform.position.x <= _fEndpointPosX)
				{
					_atkMachineUsecase.SetTarget(_player.transform);
				}

				_atkMachineUsecase.Update();
			}
		}

	}
	private void Update()
	{
		
		if (GameManager.Instance.GetIsPlayerSpawned() == true)
			FindTarget(GameManager.Instance.GetPlayerTransform(), _fRange);
		
	}

	private void LateUpdate()
	{
		if (_enemyState != null && _isInitState)
			_enemyState.LateUpdate();
	}
	public virtual void ReturnToPool()
	{
		GameManager.Instance.RemoveDieEnemy(transform);
		gameObject.SetActive(false);

		//StartCoroutine(CoDestroy());
	}
	public virtual void SetPosition(UnityEngine.Vector2 position)
	{
		transform.position = position;
	}

	public UnityEngine.Vector2 GetPosition()
	{
		return transform.position;
	}

	public Transform GetTransform()
	{
		return transform;
	}
	public float GetMoveSpeed()
	{
		return _fMoveSpeed;
	}
	public void SetMoveSpeed(float speed)
	{
		_fMoveSpeed = speed;
	}
	#endregion

	/// <summary> 에너미 체력바 시작하는 함수 </summary>
	private void InitHpBar()
	{
		_hpView.DoFill(1, false);
		_hpView.Disable();
	}

	public void SwitchState(ActorEnum.State state)
	{
		switch (state)
		{
			case ActorEnum.State.Idle:
				_enemyState.ChangeState<EnemyStateIdle>();
				break;
			case ActorEnum.State.Walk:
				_enemyState.ChangeState<EnemyStateWalk>();
				break;
			case ActorEnum.State.Attack:
				_enemyState.ChangeState<EnemyStateAttack>();
				break;
			case ActorEnum.State.Die:
				_enemyState.ChangeState<EnemyStateDie>();
				break;
		}
	}

	public void TakeDamage(BigInteger damage)
	{
		if(transform != null)
		{
			_bCurrentHp -= damage;

			if (_bCurrentHp <= 0)
			{
				_bCurrentHp = 0;
				_hpView.DoFill((float)_bCurrentHp / (float)_bMaxHp);
				SwitchState(ActorEnum.State.Die);
				_hpView.Disable();
				return;
			}


			_hpView.DoFill((float)_bCurrentHp / (float)_bMaxHp);
		}
		
	}
	public void EnemyDieEffect()
	{
		var prefabFx = PoolFxExtension.GetPool(PoolEnums.PoolFxId.DestroyParts_01);

		prefabFx.position = GetEnemyHitNode().position;

		prefabFx.gameObject.SetActive(true);
	}
	public void Disable()
	{
		gameObject.SetActive(false);
	}
	#region Getter & Setter
	public Animator GetAnimator()
	{
		return _animator;
	}

	public Player GetTargetPlayer()
	{
		return _player;
	}

	public int GetEnemyRange()
	{
		return _fRange;
	}

	public Transform GetEnemyHitNode()
	{
		return _trHitNode;
	}
	public Transform GetEnemyAttackNode()
	{
		return _trAttackNode;
	}
	public AnimationHelper GetAnimationHelper()
	{
		return _animationHelper;
	}
	#endregion
}
