using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class BulletBody : NetworkBehaviour
{
    public NetworkVariable<int> markColorIndex = new NetworkVariable<int>();
    public List<Renderer> bulletMarkRenderers;

    public BattleBehavior battleBehavior;
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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsClient)
        {
            UpdatePlayerColor();
        }
    }
    
    private void UpdatePlayerColor()
    {
        Color bulletMarkColor = playerColors[markColorIndex.Value];
        foreach (Renderer markRenderer in bulletMarkRenderers)
        {
            markRenderer.material.SetColor("_BulletColor", bulletMarkColor);
            markRenderer.material.SetColor("_LightColor", bulletMarkColor);
        }
    }
}
