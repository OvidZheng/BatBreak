using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class BattleBehavior : NetworkBehaviour
{
    public GameObject bulletPrefab; // 子弹的预制体
    public Vector3 bulletPosOffset;
    public int maxBullets = 10; // 最大子弹数
    public float fireRate = 5.0f; // 每秒最大射速

    public float bulletRecoveryRate = 1; // 每秒恢复的子弹数
    public NetworkVariable<bool> fireLock = new NetworkVariable<bool>(false);
    private float nextBulletRecoveryTime = 0; // 下一次恢复子弹的时间

    public NetworkVariable<int> currentBullets = new NetworkVariable<int>(); // 当前子弹数
    public NetworkVariable<int> currentPower = new NetworkVariable<int>(); // 当前子弹数
    public int maxPower = 30;
    public float clearRadius = 5.0f; // 清除子弹的范围半径
    public int ClearPowerToConsume = 1; // 消耗的子弹数
    public int PowerRestorePerBulletPassby;
    public GameObject clearEffectPrefab; // 清除效果的预制体
    public float effectDuration = 0.5f; // 效果持续时间
    public PlayerOutlookController playerOutlookController;
    public int passbyFlashCount;
    public float passbyFlashDuration;
    private float nextFireTime = 0.0f; // 下次允许射击的时间

    private void Start()
    {
        if (IsServer)
        {
            currentBullets.Value = maxBullets; // 初始时满子弹
            currentPower.Value = maxPower;
        }
    }

    private void Update()
    {
        if (fireLock.Value || !IsOwner)
        {
            return;
        }
        // 检查是否到达了可以射击的时间
        if (Time.time > nextFireTime)
        {
            // 鼠标左键被按下
            if (Input.GetMouseButton(0) && currentBullets.Value > 0)
            {
                FireBulletServerRpc();
                nextFireTime = Time.time + 1 / fireRate;
            }
        }

        
        // 清空子弹
        if (Input.GetMouseButtonDown(1))
        {
            ClearBulletsInRangeServerRpc();
        }
    }

    private void FixedUpdate()
    {
        if (fireLock.Value)
        {
            return;
        }
        
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
            BulletBody bulletBody = bullet.GetComponent<BulletBody>();

            if (bulletNetworkObject != null && bulletBody != null)
            {
                bulletNetworkObject.Spawn();
                bulletBody.markColorIndex.Value = playerOutlookController.markColorIndex.Value;
                currentBullets.Value--; // 射击后减少子弹
            }
            else
            {
                Debug.LogError("Spawned bullet does not have a NetworkObject component.");
            }
        }
    }
    
    [ServerRpc]
    void ClearBulletsInRangeServerRpc()
    {
        if (!IsServer || currentPower.Value < ClearPowerToConsume) return;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, clearRadius);
        foreach (var hitCollider in hitColliders)
        {
            var bullet = hitCollider.GetComponent<BulletBehaviour>();
            if (bullet != null)
            {
                bullet.DestroySelf();
            }
        }
        currentPower.Value -= ClearPowerToConsume; // 消耗子弹
        CreateClearEffectClientRpc();
    }
    
    
    [ClientRpc]
    void CreateClearEffectClientRpc()
    {
        StartCoroutine(CreateClearEffectCoroutine());
    }
    public int GetCurrentBullets()
    {
        return currentBullets.Value;
    }
    
    private IEnumerator CreateClearEffectCoroutine()
    {
        GameObject effect = Instantiate(clearEffectPrefab, transform.position, Quaternion.identity);
        float startTime = Time.time;

        while (Time.time < startTime + effectDuration)
        {
            float scale = Mathf.Lerp(0, clearRadius * 1.2f, (Time.time - startTime) / effectDuration);
            effect.transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }

        Destroy(effect);
    }

    public void AddPowerBulletPassby()
    {
        if (!IsServer)
        {
            return;
        }
        currentPower.Value = Mathf.Min(maxPower, currentPower.Value + PowerRestorePerBulletPassby);
        CreateAddPowerEffectClientRpc();
    }
    
    [ClientRpc]
    void CreateAddPowerEffectClientRpc()
    {
        playerOutlookController.PlayerHealthRendererFlash(passbyFlashDuration ,passbyFlashCount);
    }
}
