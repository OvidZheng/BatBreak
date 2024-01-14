using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerBody : NetworkBehaviour
{
    public NetworkVariable<int> Health = new NetworkVariable<int>(100);
    public NetworkVariable<int> markColorIndex = new NetworkVariable<int>(0); // 用于同步颜色的网络变量
    public List<Renderer> playerMarkRenderers; // 修改为Renderer的列表

    
    private Renderer playerRenderer; // Renderer to change the color of the player


    // 定义一个颜色数组
    // 定义一个颜色数组
    private Color[] playerColors = new Color[] {
        new Color(0, 0, 1, 4), // 亮蓝色
        new Color(1, 1, 0, 4), // 亮黄色
        new Color(1.5f, 0, 1.5f, 4), // 亮紫色
        new Color(0, 1.5f, 1.5f, 4), // 亮青色
        new Color(1.5f, 0.75f, 0, 4), // 亮橙色
        new Color(1.5f, 1.125f, 1.2f, 4), // 亮粉色
        new Color(0.75f, 1.125f, 1.5f, 4), // 亮天蓝色
        new Color(0.9f, 1.5f, 0.9f, 4), // 亮薄荷绿
        new Color(1.5f, 1.2f, 0.9f, 4) // 亮桃色
    };




    private void Start()
    {
        // Get the Renderer component from the player game object
        playerRenderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        if (IsClient)
        {
            UpdatePlayerColor();
        }
    }

    public void TakeDamage(int damage)
    {
        if (IsServer)
        {
            Health.Value -= damage;

            if (Health.Value <= 0)
            {
                // 玩家死亡的逻辑（例如重新生成等）
                GetComponent<NetworkObject>().Despawn();
                Destroy(gameObject);
            }
        }

    }
    
    private void ChangeColor()
    {
        if (IsClient)
        {
            // 请求服务器更改颜色
            RequestChangeMarkColorServerRpc();
        }
    }
    
    [ServerRpc]
    private void RequestChangeMarkColorServerRpc(ServerRpcParams rpcParams = default)
    {
        markColorIndex.Value = (markColorIndex.Value + 1) % playerColors.Length;
    }


    private void UpdatePlayerColor()
    {
        // Map the health to a color value (green to red)
        float healthPercentage = Health.Value / 100f;
        Color playerHealthColor = Color.Lerp(Color.red, Color.green, healthPercentage);
        playerRenderer.material.SetColor("_PlayerBaseColor", playerHealthColor);
        Color playerMarkColor = playerColors[markColorIndex.Value];
        // 更新每个标记的颜色
        foreach (Renderer markRenderer in playerMarkRenderers)
        {
            markRenderer.material.SetColor("_PlayerBaseColor", playerMarkColor);
        }
    }
    
    private void OnEnable()
    {
        NetworkManagerUI.OnColorChangeRequested += HandleColorChange;
    }

    private void OnDisable()
    {
        NetworkManagerUI.OnColorChangeRequested -= HandleColorChange;
    }
    
    private void HandleColorChange()
    {
        if (IsOwner) // 确保只有当前客户端的玩家执行这个操作
        {
            RequestChangeColor();
        }
    }

    public void RequestChangeColor()
    {
        if (IsClient)
        {
            // 请求服务器更改颜色
            RequestChangeMarkColorServerRpc();
        }
    }
}