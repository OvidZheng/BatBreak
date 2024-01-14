using Unity.Netcode;
using UnityEngine;

public class DestructibleObstacle : NetworkBehaviour
{
    public NetworkVariable<int> Health = new NetworkVariable<int>();
    private int maxHealth;
    public GameObject destructionEffectPrefab; // 摧毁时的粒子特效预制体
    private Renderer objRenderer; // 对象的渲染器

    void Start()
    {
        objRenderer = GetComponent<Renderer>();
        maxHealth = Health.Value; // 假设初始生命值是最大生命值
        UpdateMaterialColor(); // 初始颜色更新
    }

    public void TakeDamage(int damage)
    {
        if (!IsServer) return;

        Health.Value -= damage;
        UpdateMaterialColor(); // 更新颜色

        if (Health.Value <= 0)
        {
            PlayDestructionEffectClientRpc();
            DestroySelf();
        }
    }

    private void UpdateMaterialColor()
    {
        float healthPercentage = (float)Health.Value / maxHealth;
        Color newColor = Color.Lerp(Color.black, Color.white, healthPercentage); // 白色到黑色
        objRenderer.material.SetColor("_Color", newColor);
    }

    [ClientRpc]
    private void PlayDestructionEffectClientRpc()
    {
        if (destructionEffectPrefab != null)
        {
            Instantiate(destructionEffectPrefab, transform.position, Quaternion.identity);
        }
    }

    private void DestroySelf()
    {
        NetworkObject networkObject = GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.Despawn(); // 在服务器上销毁障碍物，并在所有客户端上同步
            Destroy(gameObject);
        }
    }
}