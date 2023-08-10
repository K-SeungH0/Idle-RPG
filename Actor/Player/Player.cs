using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Cysharp.Threading.Tasks;

//using UnityEngine.UIElements;
//using static EnemyEnum;
//using static UnityEditor.PlayerSettings;

public class Player : MonoBehaviour, IActor
{
    [SerializeField]
    private PlayerStats _stats;
	[Header("HPBar")]
	[SerializeField]
	protected HpView _hpView;
	private Transform _target;

	[Header("Animation")]
	[SerializeField]
	private Animator _animator;
	[SerializeField]
	private AnimationHelper _animationHelper;
	/// <summary> Player FSM </summary>
	private StateMachine<Player> _playerState;

	[SerializeField]
	private Animation animation_source;

	private Coroutine Co_HandleAnimation;
	/// <summary> 플레이어 현재 상태를 나타내는 프로퍼티 </summary>
	public ActorEnum.State _enCurrentState { get; set; }

	private bool _isInit;
	private bool _isInitState;

	[SerializeField]
	private Transform _playerNodeHit;

	private BigInteger _currentHp;

	private bool _isStatLoaded = false;

	private IAttackMachineUsecase _atkMachineUsecase = new AttackMachineUsecase();
	private IEquipmentSystemUsecase _equipmentSystemUsecase = new EquipmentSystemUsecase();

	/// <summary> 플레이어 스킬 쿨타임 보관하는 리스트 </summary>
	private List<float> _skillCoolTimeList = new List<float>();

	public System.Action GoldChange;  
	public System.Action GemChange;
    public void Init()
	{
		if (_isInit == false)
		{
			BaseStatsLoad();
			InitAttackMachine();
            _equipmentSystemUsecase.Init();
			HPRecovery().Forget();
            GoldChange += FindObjectOfType<MainTopUI>().UpdateGold;
            GemChange += FindObjectOfType<MainTopUI>().UpdateGem;
            _isInit = true;
		}
	}
	public void InitCurrentHP()
	{
		SetCurrentHp(_stats.BHp + _stats.BStatsHp);
    }
	public void SwitchState(ActorEnum.State state)
	{
		if (_isInitState == false)
			return;

		switch (state)
		{
			case ActorEnum.State.Idle:
				_playerState.ChangeState<PlayerStateIdle>();
				break;
			case ActorEnum.State.Walk:
				_playerState.ChangeState<PlayerStateWalk>();
				break;
			case ActorEnum.State.Attack:
				_playerState.ChangeState<PlayerStateAttack>();
				break;
			case ActorEnum.State.Die:
				_playerState.ChangeState<PlayerStateDie>();
				break;
			case ActorEnum.State.Back:
				_playerState.ChangeState<PlayerStateBack>();
				break;
		}
	}

	public UnityEngine.Vector2 GetPosition()
	{
		return transform.position;
	}

	public Transform GetTransform()
	{
		return transform;
	}


	public void Enable()
	{
		transform.gameObject.SetActive(true);
		InitHpBar();
	}

	private void InitHpBar()
	{
		_hpView.DoFill(1, false);
		_hpView.Disable();
	}


	/// <summary> 플레이어 맞는 모션 재생 </summary>
	public void PlayAnimation(string animation_name, System.Action OnCompleted, bool is_check_end_animation = false)
	{
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
	/// <summary> 어택머신 초기 설정하는 함수 </summary>
	public void InitAttackMachine()
	{
		_atkMachineUsecase.Init();
		
		_atkMachineUsecase.Add(0, 0f, 1f);
		_atkMachineUsecase.Add(1, 0f, 5f);
		_atkMachineUsecase.Add(2, 0f, 10f);
		_atkMachineUsecase.Add(3, 0f, 10f);
		_atkMachineUsecase.Add(4, 5f, 15f);
		_atkMachineUsecase.Add(5, 0f, 15f);
		_atkMachineUsecase.Add(6, 0f, 20f);

		_skillCoolTimeList.Add(1f);
		_skillCoolTimeList.Add(5f);
		_skillCoolTimeList.Add(10f);
		_skillCoolTimeList.Add(10f);
		_skillCoolTimeList.Add(15f);
		_skillCoolTimeList.Add(15f);
		_skillCoolTimeList.Add(20f);

		_atkMachineUsecase.SkillCallBack(OnSkillCallback);
	}

	public void InitState()
	{
		if(!_isInitState)
		{
			_playerState = new StateMachine<Player>(this);

			_playerState.AddState(new PlayerStateIdle());
			_playerState.AddState(new PlayerStateAttack());
			_playerState.AddState(new PlayerStateDie());
			_playerState.AddState(new PlayerStateWalk());
			_playerState.AddState(new PlayerStateBack());

			_isInitState = true;
		}
	}
	private void BaseStatsLoad()
	{
		var baseData = AppManager.Instance.GetDataTableManager().GetBaseDataTable().GetDataTable();
		_stats = new PlayerStats()
		{
			BAtk			= baseData[enStatusElement.Atk].value,
			BAtkPer			= baseData[enStatusElement.AtkPercent].value,
			fAtkSpd			= baseData[enStatusElement.AtkSpeed].value,
			fAtkSpdPer		= baseData[enStatusElement.AtkSpeedPercent].value,
			fDoubleShot		= baseData[enStatusElement.DoubleShotPercent].value,
			fTripleShot		= baseData[enStatusElement.TripleShotPercent].value,
			nAtkRange		= baseData[enStatusElement.AtkRange].value,
			nAtkRangePer	= baseData[enStatusElement.AtkRangePercent].value,
			BHp				= baseData[enStatusElement.Hp].value,
			BHpPer			= baseData[enStatusElement.HpPercent].value,
			BHpRecovery		= baseData[enStatusElement.HpRecovery].value,
            BHpRecoveryPer	= baseData[enStatusElement.HpRecoveryPercent].value,
			fCrit			= baseData[enStatusElement.CritRatePercent].value,
			BCritDmg		= baseData[enStatusElement.CritDmgPercent].value,
			nMoveSpd		= baseData[enStatusElement.MoveSpeed].value,
			nMoveSpdPer		= baseData[enStatusElement.MoveSpeedPercent].value,
			fDodge			= baseData[enStatusElement.DodgePercent].value,
			BGoldGain		= baseData[enStatusElement.GoldAcquisitionAmountPercent].value,
			BRewardGain		= baseData[enStatusElement.IncreaseRewardAmountPercent].value,
        };

		_currentHp = _stats.BHp;
		_stats.nAtkRange = 7;
		_isStatLoaded = true;
		
    }

	/// <summary>
	/// 공격 속도가 바뀌었을때 호출 되는 함수
	/// </summary>
	public void ChangeAttackSpeed()
	{
        _atkMachineUsecase.Remove(0);

		float atkSpd = _stats.fAtkSpd * (1 + _stats.fStatsAtkSpd);
        _atkMachineUsecase.Add(0, 0f, 1 / atkSpd);
    }
    public void AttackMachineManualSkill(int slotIndex)
	{
		_atkMachineUsecase.SetManualSkillSlot(slotIndex);
	}

	private void Update()
	{
		if (GameManager.Instance.GetIsGameOver() == false)
		{
			if (_isStatLoaded == false)
			{
				return;
			}


			FindTarget(GameManager.Instance.GetEnemyList(), _stats.nAtkRange);
			_atkMachineUsecase.SetTarget(_target);
			_atkMachineUsecase.Update();
		}
		

		if(Input.GetKeyDown(KeyCode.F))
		{
			//_currentHp = 0;
            SetCurrentHp(-_currentHp);
        }
		if(Input.GetKeyDown(KeyCode.G))
		{
			GainGold(9999999999);
		}

    }
	private void LateUpdate()
	{
		if (_playerState != null)
			_playerState.LateUpdate();
	}


	/// <summary> Actor들이 Target을 찾을 때 사용하는 함수 </summary>
	/// <param name="targetList"></param>
	/// <param name="distance"></param>
	int _nFindCount = 0;
	//public void FindTarget(List<Enemy> targetList, float distance)
	//{
	//	if(targetList.Count == 0)
	//	{
	//		_target = null;
	//		return;
	//	}
	//	Transform target = null;
	//	float[] distances = new float[targetList.Count];

	//	for (int i = 0; i < targetList.Count; i++)
	//	{
	//		distances[i] = UnityEngine.Vector2.Distance(transform.position, targetList[i].transform.position);
	//	}

	//	if (targetList.Count > 0)
	//	{
	//		int minIndex = 0;
	//		float minDistance = distances[0];

	//		for (int i = 1; i < distances.Length; i++)
	//		{
	//			if (distances[i] < minDistance)
	//			{
	//				minIndex = i;
	//				minDistance = distances[i];
	//			}
	//		}

	//		// 7은 임시 사정거리
	//		if (distances[0] < distance)
	//		{
	//			target = targetList[minIndex].transform;

	//		}
	//		else
	//		{
	//			target = null;

	//		}
	//		_target = target;
	//		GameManager.Instance.SetEnemyTarget(_target);
	//	}
	//	else
	//	{
	//		_target = null;
	//	}

		
	
	//}

	public void FindTarget(List<Enemy> targetList, float distance)
	{
		if (targetList.Count == 0)
		{
			_target = null;
			GameManager.Instance.SetEnemyTarget(_target);
			return;
		}

		float[] distances = new float[targetList.Count];

		for (int i = 0; i < targetList.Count; i++)
		{
			distances[i] = UnityEngine.Vector2.Distance(transform.position, targetList[i].transform.position);
		}

		int minIndex = 0;
		float minDistance = distances[0];

		for (int i = 1; i < distances.Length; i++)
		{
			minDistance = Mathf.Min(minDistance, distances[i]);
			if (distances[i] == minDistance)
			{
				minIndex = i;
			}
		}

		Transform target = (distances[minIndex] < distance) ? targetList[minIndex].transform : null;

		_target = target;
		GameManager.Instance.SetEnemyTarget(_target);
	}


	private void OnSkillCallback(List<int> skillCall)
	{
		if(_target != null)
		{
			for (int i = 0; i < skillCall.Count; i++)
			{
				SkillStart(skillCall[i]);
			}
		}
		
	}
	private void SkillStart(int slotIndex)
	{
		
		
		//LogManager.Instance.PrintLog(LogManager.enLogType.Normal, "Player Skill ID : " + skillId.ToString(), LogManager.LogColor.Magenta);
		// 여기는 사실 스킬 매니저 같은게 있으면 좋을듯...
		if (slotIndex == 0)
		{
			SwitchState(ActorEnum.State.Attack);
			
			//LogManager.Instance.PrintLog(LogManager.enLogType.Normal, "플레이어 기본공격!!", LogManager.LogColor.Magenta);
		}
		else if(slotIndex == 1)
		{
			//LogManager.Instance.PrintLog(LogManager.enLogType.Normal, "플레이어 수류탄 던지기!!", LogManager.LogColor.Magenta);
			SwitchState(ActorEnum.State.Attack);
			var projectile = PoolProjectileExtension.GetPool(PoolEnums.PoolProjectileId.Skill001);
			if (projectile != null)
			{
				ProjectileStats projectileStat = new ProjectileStats();
				projectileStat.ElapsedTime = 0f;
				projectileStat.Target = _target;
				projectileStat.Start = transform;
				projectileStat.Speed = 4.0f;
				projectileStat.Damage = 50;
				projectileStat.DamageTargetType = ProjectileStats.enDamageTarget.Enemy;
				projectile.gameObject.GetComponent<ParabolicProjectile>().SetProjectile(projectileStat);
			}
		}
		else if (slotIndex == 2)
		{
			//LogManager.Instance.PrintLog(LogManager.enLogType.Normal, "플레이어 나사던지기!!", LogManager.LogColor.Magenta);
			SwitchState(ActorEnum.State.Attack);
			for (int i = 0; i < 5; i++)
			{
				var projectile = PoolProjectileExtension.GetPool(PoolEnums.PoolProjectileId.Skill002);
				if (projectile != null)
				{
					ProjectileStats projectileStat = new ProjectileStats();
					projectileStat.ElapsedTime = 0f;
					projectileStat.Target = _target;
					projectileStat.Start = transform;
					projectileStat.Speed = 5.0f;
					projectileStat.Damage = 100;
					projectileStat.DamageTargetType = ProjectileStats.enDamageTarget.Enemy;
					projectile.gameObject.GetComponent<NormalProjectile>().SetProjectile(projectileStat);
				}
			}
			
		}
		else if (slotIndex == 3)
		{
			var prefabFx = PoolFxExtension.GetPool(PoolEnums.PoolFxId.Skill005_Hit);
			UnityEngine.Vector2 pos = new UnityEngine.Vector2(_playerNodeHit.position.x, _playerNodeHit.position.y);
			prefabFx.position = pos;
			prefabFx.gameObject.SetActive(true);

			SetCurrentHp(100);

			//_currentHp += 100;
			//if (_currentHp >= _stats.BHp)
			//	_currentHp = _stats.BHp;
			//_hpView.DoFill((float)_currentHp / (float)_stats.BHp);
			//LogManager.Instance.PrintLog(LogManager.enLogType.Normal, "플레이어 고쳐 먹기!!", LogManager.LogColor.Magenta);
		}
		else if (slotIndex == 4)
		{
			var projectile = PoolProjectileExtension.GetPool(PoolEnums.PoolProjectileId.Skill006);
			if (projectile != null)
			{
				ProjectileStats projectileStat = new ProjectileStats();
				projectileStat.ElapsedTime = 0f;
				projectileStat.Target = _target;
				projectileStat.Start = transform;
				projectileStat.Speed = 5.0f;
				projectileStat.RemainTime = 5.0f;
				projectileStat.Damage = 10;
				projectileStat.DamageTargetType = ProjectileStats.enDamageTarget.Enemy;
				projectile.gameObject.GetComponent<DronBombingProjectile>().SetProjectile(projectileStat);
			}
			//LogManager.Instance.PrintLog(LogManager.enLogType.Normal, "플레이어 드론소환술!!", LogManager.LogColor.Magenta);
		}
		else if (slotIndex == 5)
		{
			StartCoroutine(CreateMissileProjectilesWithDelay());
			//LogManager.Instance.PrintLog(LogManager.enLogType.Normal, "플레이어 미사일포격해버리기!!", LogManager.LogColor.Magenta);
		}
		else if (slotIndex == 6)
		{

			//LogManager.Instance.PrintLog(LogManager.enLogType.Normal, "플레이어 파괴광선!!!!!", LogManager.LogColor.Magenta);
			SwitchState(ActorEnum.State.Attack);
			var projectile = PoolProjectileExtension.GetPool(PoolEnums.PoolProjectileId.Skill008);
			if (projectile != null)
			{
				ProjectileStats projectileStat = new ProjectileStats();
				projectileStat.ElapsedTime = 0f;
				projectileStat.Target = null;
				projectileStat.Start = transform;
				projectileStat.Speed = 2.0f;
				projectileStat.Damage = 5;
				projectileStat.DamageTargetType = ProjectileStats.enDamageTarget.NonTarget;
				projectile.gameObject.GetComponent<DestructionLaserProjectile>().SetProjectile(projectileStat);
			}
		}





		if (slotIndex != 0)
		{
			var skillListUI = GameManager.Instance.GetSkillListUI();
			skillListUI.SkillCoolTimeBySlotIndex(slotIndex - 1, _skillCoolTimeList[slotIndex]);
		}

	}
	IEnumerator CreateMissileProjectilesWithDelay()
	{

		for (int i = 0; i < 10; i++)
		{
			yield return new WaitForSeconds(0.1f); // Delay for 0.5 seconds

			var projectile = PoolProjectileExtension.GetPool(PoolEnums.PoolProjectileId.Skill007);
			if (projectile != null)
			{
				ProjectileStats projectileStat = new ProjectileStats();
				projectileStat.ElapsedTime = 0f;
				projectileStat.Target = _target;
				projectileStat.Start = GameManager.Instance.GetMissileRainSpotPoint();
				projectileStat.Speed = 5.0f;
				projectileStat.Count = i;
				projectileStat.Damage = 5;
				projectileStat.DamageTargetType = ProjectileStats.enDamageTarget.Enemy;
				projectile.gameObject.GetComponent<MissileBombingProjectile>().SetProjectile(projectileStat);
			}
		}
	}
	public void ReturnToPool()
	{
		Destroy(gameObject);
	}
	
    public void TakeDamage(BigInteger damage)
	{

		SetCurrentHp(-damage);
		//_currentHp -= damage;

		//if (_currentHp <= 0)
		//	_currentHp = 0;

		//_hpView.DoFill((float)_currentHp / (float)_stats.BHp);

		if (_currentHp == 0)
		{
			SwitchState(ActorEnum.State.Die);
			GameManager.Instance.GameOver();
		}

	}

	public void UseGold(BigInteger gold)
	{
        _stats.BGold -= gold;
        GoldChange?.Invoke();
    }
    public void GainGold(BigInteger gold)
	{
		_stats.BGold += gold;
        GoldChange?.Invoke();
    }
	public void GainGem(BigInteger gem) 
	{
		_stats.BGem += gem;
		GemChange?.Invoke();
	}
	public void Disable()
	{
		gameObject.SetActive(false);
	}

	#region Getter & Setter

	public float GetPlayerAttackRange()
	{
		return UnityEngine.Vector2.Distance(_target.position, transform.position);
	}

	public Transform GetTarget()
	{
		return _target;
	}

	public Animator GetAnimator()
	{
		return _animator;
	}
	public void SetPosition(UnityEngine.Vector2 position)
	{
		transform.position = position;
	}

	public void SetMoveSpeed(float speed)
	{

	}
	private async UniTaskVoid HPRecovery()
	{
		while (true)
		{
            await UniTask.Delay(System.TimeSpan.FromSeconds(1));
			SetCurrentHp(_stats.BHpRecovery + (_stats.BHpRecovery * _stats.BHpRecoveryPer / 100));
   //         _currentHp += _stats.BHpRecovery + (_stats.BHpRecovery * _stats.BHpRecoveryPer / 100);
			//if (_currentHp > _stats.BHp)
			//	_currentHp = _stats.BHp;

   //         _hpView.DoFill((float)_currentHp / (float)_stats.BHp);
        }
	}
	public AnimationHelper GetAnimationHelper()
	{
		return _animationHelper;
	}
	public IEquipmentSystemUsecase GetEquipmentSystemUsecase()
	{
		return _equipmentSystemUsecase;
	}

	public PlayerStats GetPlayerStats()
	{
		return _stats;
	}

    public void SetCurrentHp(BigInteger hp)
    {
        _currentHp += hp;
        if (_currentHp <= 0)
            _currentHp = 0;
        else if (_currentHp >= _stats.BHp + _stats.BStatsHp)
            _currentHp = _stats.BHp + _stats.BStatsHp;

        _hpView.DoFill((float)_currentHp / (float)_stats.BHp);
		_hpView.Enable();
    }
    #endregion
}
