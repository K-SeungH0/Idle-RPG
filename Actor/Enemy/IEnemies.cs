using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemies
{
	void Init(EnemyData data);
	void ReturnToPool();
	void SetPosition(Vector2 position);
	void SetSizeDelta(Transform transform);
	void SetMoveSpeed(float speed);
	Vector2 GetPosition();
	Transform GetTransform();
}
