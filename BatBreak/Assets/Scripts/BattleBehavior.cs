using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BattleBehavior : NetworkBehaviour
{
    public GameObject bulletPrefab; // 子弹的预制体
    public Vector3 bulletPosOffset;
    public int maxBullets = 10; // 最大子弹数
    public float bulletRecoveryRate = 1; // 每秒恢复的子弹数
    private float nextBulletRecoveryTime = 0; // 下一次恢复子弹的时间

    public NetworkVariable<int> currentBullets = new NetworkVariable<int>(); // 当前子弹数

    private void Start()
    {
        if (IsServer)
        {
            currentBullets.Value = maxBullets; // 初始时满子弹
        }
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }
            // 发射子弹
        if (Input.GetKeyDown(KeyCode.J) && currentBullets.Value > 0)
        {
            FireBulletServerRpc();
        }
    }

    private void FixedUpdate()
    {
        if (IsServer)
        {
            // 子弹恢复机制
            if (IsServer && currentBullets.Value < maxBullets && Time.fixedTime >= nextBulletRecoveryTime)
            {
                nextBulletRecoveryTime = Time.fixedTime + 1 / bulletRecoveryRate;
                if (currentBullets.Value < maxBullets)
                currentBullets.Value++; // 恢复一发子弹
            }
        }
    }

    [ServerRpc]
    void FireBulletServerRpc()
    {
        if (!IsServer) return;

        if (currentBullets.Value > 0)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.TransformPoint(bulletPosOffset), transform.rotation);
            NetworkObject bulletNetworkObject = bullet.GetComponent<NetworkObject>();

            if (bulletNetworkObject != null)
            {
                bulletNetworkObject.Spawn();
                currentBullets.Value--; // 射击后减少子弹
            }
            else
            {
                Debug.LogError("Spawned bullet does not have a NetworkObject component.");
            }
        }
    }

    public int GetCurrentBullets()
    {
        return currentBullets.Value;
    }
}
