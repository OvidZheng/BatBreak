using System;
using Unity.Netcode;
using UnityEngine;

public class DestructibleObstacle : NetworkBehaviour
{
    private NetworkVariable<int> Health = new NetworkVariable<int>();
    public int maxHealth;
    public GameObject destructionEffectPrefab; // 摧毁时的粒子特效预制体
    private Renderer objRenderer; // 对象的渲染器
    private bool isDestoyed = false;
    public GameObject[] pickableItemPrefabs; // 可拾取物品预制体的公共列表
    public float itemSpawnProbability = 0.5f; // 生成物品的概率（0到1之间）


    void Start()
    {
        objRenderer = GetComponent<Renderer>();
        if (IsServer)
        {
            Health.Value = maxHealth; // 假设初始生命值是最大生命值
        }
    }

    public void TakeDamage(int damage)
    {
        if (!IsServer) return;

        Health.Value -= damage;

        if (Health.Value <= 0 && !isDestoyed)
        {
            PlayDestructionEffectClientRpc();
            if (UnityEngine.Random.value < itemSpawnProbability) // 使用概率判断是否生成物品
            {
                SpawnPickableItem(); // 在所有客户端生成可拾取物品
            }
            DestroySelf();
        }
    }

    private void UpdateMaterialColor()
    {
        float healthPercentage = (float)Health.Value / maxHealth;
        Color minColor = new Color(0.2f, 0.2f, 0.2f); // 设定一个基础亮度，例如深灰色
        Color maxColor = Color.white; // 最大生命值时的颜色为白色
        Color newColor = Color.Lerp(maxColor, minColor, healthPercentage); // 从基础亮度到白色之间插值
        objRenderer.material.SetColor("_Color", newColor);
    }



    private void FixedUpdate()
    {
        UpdateMaterialColor(); // 更新颜色
    }

    [ClientRpc]
    private void PlayDestructionEffectClientRpc()
    {
        if (destructionEffectPrefab != null)
        {
            Instantiate(destructionEffectPrefab, transform.position, Quaternion.identity);
        }
    }
    
    private void SpawnPickableItem()
    {
        if (pickableItemPrefabs.Length > 0)
        {
            int index = UnityEngine.Random.Range(0, pickableItemPrefabs.Length);
            GameObject item = Instantiate(pickableItemPrefabs[index], transform.position, Quaternion.identity);
            NetworkObject networkItem = item.GetComponent<NetworkObject>();
            if (networkItem != null)
            {
                networkItem.Spawn(); // 网络实例化物品
            }
        }
    }

    private void DestroySelf()
    {
        if (isDestoyed)
        {
            return;
        }

        isDestoyed = true;
        NetworkObject networkObject = GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.Despawn(); // 在服务器上销毁障碍物，并在所有客户端上同步
            Destroy(gameObject);
        }
    }
}