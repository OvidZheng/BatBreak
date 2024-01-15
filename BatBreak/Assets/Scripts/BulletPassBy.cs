using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPassBy : MonoBehaviour
{
    public BattleBehavior battleBehavior;
    private void OnTriggerExit(Collider other)
    {
        // 检查退出触发器的物体是否在 'bullet' 图层
        if (other.gameObject.layer == LayerMask.NameToLayer("Bullet"))
        {
            battleBehavior.AddPowerBulletPassby();
            Debug.Log("pssby");
        }
    }
}
