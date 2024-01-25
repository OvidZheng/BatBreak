using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace BehaviourTree.Task.Action
{
    public class TurnTowardsM : BehaviorDesigner.Runtime.Tasks.Action
    {
        public SharedGameObject targetObject; // 目标游戏对象
        public float rotationSpeed = 2.0f; // 旋转速度
        private UnityEngine.AI.NavMeshAgent navMeshAgent;
        public override TaskStatus OnUpdate()
        {
            if (targetObject.Value == null)
            {
                return TaskStatus.Failure;
            }

            // 计算新的旋转方向
            Vector3 targetDirection = targetObject.Value.transform.position - transform.position;
            targetDirection.y = 0; // 保持在同一水平面

            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            return TaskStatus.Running;
        }
    }
}