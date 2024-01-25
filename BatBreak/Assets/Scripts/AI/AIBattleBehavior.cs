using UnityEngine;
using Unity.Netcode;

public class AIBattleBehavior : NetworkBehaviour
{
    public GameObject bulletPrefab; // 子弹预制体
    public Vector3 bulletSpawnOffset; // 子弹生成的位置偏移


    public void FireBullet()
    {
        if (IsServer)
        {
            Vector3 spawnPosition = transform.position + bulletSpawnOffset;
            Quaternion spawnRotation = Quaternion.LookRotation(transform.forward);
            
            GameObject bullet = Instantiate(bulletPrefab, spawnPosition, spawnRotation);
            NetworkObject bulletNetworkObject = bullet.GetComponent<NetworkObject>();

            if (bulletNetworkObject != null)
            {
                bulletNetworkObject.Spawn();
            }
            else
            {
                Debug.LogError("Spawned bullet does not have a NetworkObject component.");
            }
        }
    }
}