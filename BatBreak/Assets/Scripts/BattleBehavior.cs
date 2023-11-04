using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleBehavior : MonoBehaviour
{
    // ... 之前的变量声明

    public GameObject bulletPrefab; // 子弹的预制体

    // ...Start () 方法

    private void Update()
    {
        // ... 之前的移动和旋转代码

        // 发射子弹
        if (Input.GetKeyDown(KeyCode.J))
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position + transform.forward, transform.rotation);
        }
    }

    // ... IsObstacleInDirection() 方法
}
