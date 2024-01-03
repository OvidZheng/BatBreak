using Unity.Netcode;
using UnityEngine;

public class BulletBehaviour : NetworkBehaviour
{
    public float bulletSpeed;
    public LayerMask obstacleMask; // 障碍物的LayerMask

    void Update()
    {
        // 仅在服务器上处理移动逻辑
        if (IsServer)
        {
            MoveBullet();
        }
    }

    // 移动子弹的方法
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
        // 检测障碍物LayerMask
        if ((obstacleMask.value & (1 << other.gameObject.layer)) > 0)
        {
            NetworkObject networkObject = GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                networkObject.Despawn(); // 在服务器上销毁子弹，并在所有客户端上同步
            }

        }
    }
}
