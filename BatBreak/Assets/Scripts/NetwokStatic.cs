using UnityEngine;
using Unity.Netcode;
using UnityEngine.Serialization;

public class NetworkStatic : NetworkBehaviour
{
    public static NetworkStatic Instance { get; private set; }

    public Transform playertrTransform;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 确保单例对象在加载新场景时不会被销毁
        }
        else
        {
            Destroy(gameObject); // 如果已经有一个实例存在，则销毁新创建的对象
        }
    }

    private void FixedUpdate()
    {
        if ((IsServer && !IsHost))
        {
            return;
        }
        if (playertrTransform == null)
        {
            FindLocalPlayer();
        }
    }
    
    private void FindLocalPlayer()
    {
        foreach (var player in FindObjectsOfType<NetworkObject>())
        {
            if (player.IsLocalPlayer)
            {
                playertrTransform = player.transform; // 修改这里获取GameObject的方式
                break;
            }
        }
    }

    public Transform GetlocalPlayerTransform()
    {
        return playertrTransform;
    }
}