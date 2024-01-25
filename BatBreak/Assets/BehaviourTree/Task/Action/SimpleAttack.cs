using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class SimpleAttack : Action
{
    public float attackDuration = 5.0f; // 攻击持续时间
    public float fireRate = 1.0f; // 每秒发射频率

    private AIBattleBehavior aiBattleBehavior;
    private float attackEndTime; // 攻击结束时间
    private float nextFireTime; // 下次发射时间

    public override void OnStart()
    {
        aiBattleBehavior = GetComponent<AIBattleBehavior>();

        // 设置攻击结束时间和下次发射时间
        attackEndTime = Time.time + attackDuration;
        nextFireTime = Time.time;
    }

    public override TaskStatus OnUpdate()
    {
        if (aiBattleBehavior == null || Time.time > attackEndTime)
        {
            // 攻击时间结束
            return TaskStatus.Success;
        }

        if (Time.time >= nextFireTime)
        {
            // 发射子弹
            aiBattleBehavior.FireBullet();
            nextFireTime = Time.time + 1 / fireRate;
        }

        // 继续攻击
        return TaskStatus.Running;
    }
}