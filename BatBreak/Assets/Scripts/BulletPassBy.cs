using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BulletPassBy : NetworkBehaviour
{
    public BattleBehavior battleBehavior;
    public PlayerOutlookController playerOutlookController;

    private void OnTriggerExit(Collider other)
    {
        if (!IsServer)
        {
            return;
        }

        // 检查退出触发器的物体是否在 'bullet' 图层
        if (other.gameObject.layer == LayerMask.NameToLayer("Bullet"))
        {
            battleBehavior.AddPowerBulletPassby();
            Debug.Log("pssby");
        }
    }
}