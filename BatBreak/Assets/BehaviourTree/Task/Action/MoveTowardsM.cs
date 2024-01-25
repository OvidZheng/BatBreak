using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;

namespace BehaviourTree.Task.Action
{
    public class MoveTowardsM : BehaviorDesigner.Runtime.Tasks.Action
    {
        public SharedGameObject targetGameobject; // 目标玩家
        public float stopDistance = 2.0f; // 停止移动的最小距离

        private NavMeshAgent navMeshAgent;

        public override void OnStart()
        {
            // 获取NavMeshAgent组件
            navMeshAgent = GetComponent<NavMeshAgent>();
            StarAgent();
            if (navMeshAgent != null && targetGameobject.Value != null)
            {
                // 设置NavMeshAgent的目标
                navMeshAgent.SetDestination(targetGameobject.Value.transform.position);
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (navMeshAgent == null || targetGameobject.Value == null)
            {
                return TaskStatus.Failure;
            }

            // 检查是否接近目标
            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= stopDistance)
            {
                // 已经接近目标，停止移动
                StopAgent();
                return TaskStatus.Success;
            }

            // 继续移动
            return TaskStatus.Running;
        }

        public override void OnEnd()
        {
            StopAgent();
        }

        public override void OnConditionalAbort()
        {
            StopAgent();
        }
        
        private void StopAgent()
        {
            if (navMeshAgent != null)
            {
                navMeshAgent.isStopped = true;
            }
        }
        
        private void StarAgent()
        {
            if (navMeshAgent != null)
            {
                navMeshAgent.isStopped = false;
            }
        }
    
    }
}