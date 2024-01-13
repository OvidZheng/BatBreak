using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public Transform[] spawnPoints; // 出生点数组
    public GameObject playerPrefab;
    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            // 选择一个出生点
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // 创建玩家对象
            GameObject player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
            
            // 在网络上生成玩家对象
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
        }
    }
}
