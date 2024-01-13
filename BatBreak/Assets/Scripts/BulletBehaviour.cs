using Unity.Netcode;
using UnityEngine;

public class BulletBehaviour : NetworkBehaviour
{
    public float bulletSpeed;
    public LayerMask obstacleMask; // 障碍物的LayerMask
    public LayerMask playerMask; // 玩家的LayerMask

    void Update()
    {
        if (IsServer)
        {
            MoveBullet();
        }
    }

    private void MoveBullet()
    {
        transform.position = transform.position + bulletSpeed * transform.forward * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer)
        {
            return;
        }

        // 检测是否命中玩家
        if ((playerMask.value & (1 << other.gameObject.layer)) > 0)
        {
            PlayerBody player = other.gameObject.GetComponent<PlayerBody>();
            if (player != null)
            {
                player.TakeDamage(10); // 假设每次命中扣除10点生命值
                DestroySelf();
            }
        }

        // 检测障碍物LayerMask
        if ((obstacleMask.value & (1 << other.gameObject.layer)) > 0)
        {
            DestroySelf();
        }
        

    }

    public void DestroySelf()
    {
        NetworkObject networkObject = GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.Despawn(); // 在服务器上销毁子弹，并在所有客户端上同步
        }
    }
}
