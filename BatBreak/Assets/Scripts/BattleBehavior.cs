using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BattleBehavior : NetworkBehaviour
{

    public GameObject bulletPrefab; // 子弹的预制体
    
    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        // 发射子弹
        if (Input.GetKeyDown(KeyCode.J))
        {
            FireBulletServerRpc();
        }
    }
    
    [ServerRpc]
    void FireBulletServerRpc()
    {
        if (!IsServer) return;

        GameObject bullet = Instantiate(bulletPrefab, transform.position + transform.forward, transform.rotation);
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
