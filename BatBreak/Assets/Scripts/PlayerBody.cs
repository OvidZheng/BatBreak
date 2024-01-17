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