using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerBody : NetworkBehaviour
{
    public NetworkVariable<int> Health = new NetworkVariable<int>(100);
    public PlayerOutlookController playerOutlookController;
    public BattleBehavior battleBehavior;
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
    
    
    public void TakeDamage(int damage)
    {
        if (IsServer)
        {
            Health.Value -= damage;
            
            if (Health.Value <= 0)
            {
                Health.Value = 0;
                // 玩家死亡的逻辑（例如重新生成等）
                // GetComponent<NetworkObject>().Despawn();
                // Destroy(gameObject);
            }
            else
            {
                battleBehavior.AddPowerBulletPassby();
            }
        }

    }
    
}