using UnityEngine;
using Unity.Netcode;

public class FirstAidKit : NetworkBehaviour
{
    public int healAmount = 20; // 恢复的生命值数量

    private void OnTriggerEnter(Collider other)
    {
        if (IsServer && other.gameObject.layer == LayerMask.NameToLayer("Player")) // 确保只在服务器上处理
        {
            PlayerBody player = other.GetComponent<PlayerBody>();
            if (player != null)
            {
                player.Heal(healAmount); // 为玩家增加生命值
                NetworkObject networkObject = GetComponent<NetworkObject>();
                if (networkObject != null)
                {
                    networkObject.Despawn(); // 在服务器上销毁FirstAidKit，并在所有客户端上同步
                    Destroy(gameObject);
                }
            }
        }
    }
}