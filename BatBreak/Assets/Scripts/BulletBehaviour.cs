using Unity.Netcode;
using UnityEngine;

public class BulletBehaviour : NetworkBehaviour
{
    public float bulletSpeed;
    public LayerMask obstacleMask; // 障碍物的LayerMask
    public LayerMask playerMask; // 玩家的LayerMask
    public int maxReflections = 3; // 子弹最多弹射的次数

    private int reflectionsCount = 0; // 当前子弹弹射的次数

    void Update()
    {
        if (IsServer)
        {
            MoveBullet();
        }
    }
    
    private void OnEnable()
    {
        GameManager.OnGameReset += DestroySelf;
    }

    private void OnDisable()
    {
        GameManager.OnGameReset -= DestroySelf;
    }


    private void MoveBullet()
    {
        transform.position += bulletSpeed * transform.forward * Time.deltaTime;
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
                return; // 提前返回，防止执行额外的逻辑
            }
        }

        // 检测并处理撞击障碍物
        if ((obstacleMask.value & (1 << other.gameObject.layer)) > 0)
        {
            
            // 新增代码：处理障碍物的损坏
            DestructibleObstacle obstacle = other.GetComponent<DestructibleObstacle>();
            if (obstacle != null)
            {
                obstacle.TakeDamage(10); // 假设每次子弹造成10点伤害
                DestroySelf();
                return;
            }
            reflectionsCount++;
            if (reflectionsCount >= maxReflections)
            {
                DestroySelf();
            }
            else
            {
                ReflectBullet(other);
            }
        }
    }

    private void ReflectBullet(Collider collider)
    {
        // 计算反射方向
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, bulletSpeed * Time.deltaTime * 10, obstacleMask))
        {
            Vector3 reflectDirection = Vector3.Reflect(ray.direction, hit.normal);
            transform.forward = reflectDirection;
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
