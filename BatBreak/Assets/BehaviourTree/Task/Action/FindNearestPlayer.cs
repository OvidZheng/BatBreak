using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace BehaviourTree.Task.Action
{
    public class FindNearestPlayer : BehaviorDesigner.Runtime.Tasks.Action
    {
        public SharedGameObject nearestPlayer; // 用来存储最近玩家的变量
        public string playerTag = "Player";

        public override TaskStatus OnUpdate()
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag(playerTag);
            float closestDistance = float.MaxValue;
            GameObject closestPlayer = null;

            foreach (var player in players)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPlayer = player;
                }
            }

            if (closestPlayer != null)
            {
                nearestPlayer.Value = closestPlayer;
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }
    }
}