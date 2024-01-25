using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace BehaviourTree.Task.Conditional
{
    public class IsTargetWithinDistance : BehaviorDesigner.Runtime.Tasks.Conditional
    {
        public SharedGameObject targetGameObject; // 目标游戏对象
        public float distance = 5.0f; // 检测距离
        public bool ignoreXAxis = false; // 是否忽略X轴
        public bool ignoreYAxis = false; // 是否忽略Y轴
        public bool ignoreZAxis = false; // 是否忽略Z轴

        public override TaskStatus OnUpdate()
        {
            if (targetGameObject.Value == null)
            {
                return TaskStatus.Failure;
            }

            Vector3 currentPosition = transform.position;
            Vector3 targetPosition = targetGameObject.Value.transform.position;

            // 根据需要忽略某些轴上的坐标
            if (ignoreXAxis) 
            {
                currentPosition.x = targetPosition.x;
            }
            if (ignoreYAxis)
            {
                currentPosition.y = targetPosition.y;
            }
            if (ignoreZAxis)
            {
                currentPosition.z = targetPosition.z;
            }

            // 计算距离并判断是否在给定范围内
            if (Vector3.Distance(currentPosition, targetPosition) <= distance)
            {
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }
        
        
        // 在编辑器中绘制检测范围
        void OnDrawGizmos()
        {
            if (targetGameObject == null || targetGameObject.Value == null)
            {
                return;
            }

            Gizmos.color = Color.yellow; // 设置Gizmos颜色
            Vector3 position = transform.position;

            // 根据忽略的轴调整位置
            if (ignoreXAxis)
            {
                position.x = targetGameObject.Value.transform.position.x;
            }
            if (ignoreYAxis)
            {
                position.y = targetGameObject.Value.transform.position.y;
            }
            if (ignoreZAxis)
            {
                position.z = targetGameObject.Value.transform.position.z;
            }

            // 绘制一个表示检测范围的球
            Gizmos.DrawWireSphere(position, distance);
        }
    }
    
}