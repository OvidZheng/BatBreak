using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class DetectPlayerInRange : Conditional
{
	public float detectRange;
	public LayerMask playerLayer;
	public SharedGameObject nearestPlayer; // 用来存储最近的玩家
	public bool writeToNearestPlayer;


	public override TaskStatus OnUpdate()
	{
		if (DetectPlayers(out Collider[] hitColliders))
		{
			// 找到最近的玩家并存储在 nearestPlayer 中
			FindNearestPlayer(hitColliders);
			return TaskStatus.Success;
		}

		return TaskStatus.Failure;
	}
	
	
	bool DetectPlayers(out Collider[] hitColliders)
	{
		// 获取指定范围内的所有碰撞体
		hitColliders = Physics.OverlapSphere(transform.position, detectRange, playerLayer);
		return hitColliders.Length > 0;
	}
	
	void FindNearestPlayer(Collider[] hitColliders)
	{
		float minDistance = Mathf.Infinity;
		GameObject closestPlayer = null;

		foreach (Collider hitCollider in hitColliders)
		{
			float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
			if (distance < minDistance)
			{
				minDistance = distance;
				closestPlayer = hitCollider.gameObject;
			}
		}

		if (writeToNearestPlayer)
		{
			nearestPlayer.SetValue(closestPlayer);
		}
	}
	
	// 可选：在编辑器中可视化检测范围
	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, detectRange);
	}
}