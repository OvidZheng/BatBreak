using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : NetworkBehaviour
{
    
    public delegate void GameModeAction();
    public static event GameModeAction OnGameReset;
    
    public Transform[] spawnPoints; 
    public GameObject playerPrefab;
    public static GameManager Instance { get; private set; }
    private List<Player> players = new List<Player>(); // 假设有一个Player类来代表玩家
    
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
    
    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisConnected;
    }
    

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisConnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            SpawnPlayerOnEnter(clientId);
        }
    }

    private void OnClientDisConnected(ulong clientId)
    {
        Player targetPlayer = null;
        foreach (Player sp in players)
        {
            if (sp.clientId == clientId)
            {
                targetPlayer = sp;
            }
        }

        if (targetPlayer != null)
        {
            DeSpawnPlayer(targetPlayer);
            players.Remove(targetPlayer);
        }

    }

    private void SpawnPlayerOnEnter(ulong clientId)
    {
        // 选择一个出生点
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        // 创建玩家对象
        GameObject newPlayerGB = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        
        Player np = new Player();
        np.playerGB = newPlayerGB;
        np.clientId = clientId;
        np.Born();
        players.Add(np);
    }

    private GameObject SpawnPlayerObj()
    {
        // 选择一个出生点
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        // 创建玩家对象
        GameObject newPlayerGB = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        return newPlayerGB;
    }

    private void DeSpawnPlayer(Player targetPlayer)
    {
        if (targetPlayer.playerGB != null)
        {
            targetPlayer.playerGB.GetComponent<NetworkObject>().Despawn();
            Destroy(targetPlayer.playerGB);
        }
    }

    private void FixedUpdate()
    {
        if (IsServer)
        {
            CheckAndResetGame();
            CachePlayerInfo();
        }
    }

    private void CachePlayerInfo()
    {
        foreach (Player sp in players)
        {
            sp.markColorIndex = sp.playerBody.markColorIndex.Value;
        }
    }
    // 检查所有玩家是否都已死亡
    private void CheckAndResetGame()
    {
        foreach (Player sp in players)
        {
            if (sp.IsAlive && sp.playerBody.Health.Value <= 0)
            {
                sp.Die();
            }
        }

        int alivePlayersCount = players.Count(player => player.IsAlive);

        // 如果场上只剩一人或者一人以下，则重置游戏
        if (alivePlayersCount <= 1)
        {
            ResetGame();
        }
    }

    // 重置游戏
    private void ResetGame()
    {
        // 重置游戏逻辑，例如重生玩家、重置环境等
        foreach (var player in players)
        {
            DeSpawnPlayer(player);
            GameObject newPlayerObj = SpawnPlayerObj();
            player.playerGB = newPlayerObj;
            player.Born(); // 假设玩家对象有一个Respawn方法
        }
        OnGameReset?.Invoke();

        // 任何其他需要的重置逻辑
    }
}

public class Player
{
    public bool IsAlive;
    public PlayerBody playerBody;
    public BattleBehavior BattleBehavior;
    public MovePlayer movePlayer;
    public GameObject playerGB;
    public ulong clientId;
    public int markColorIndex = 0;
    
    public void Born()
    {
        // 在网络上生成玩家对象
        playerGB.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
        // 玩家重生逻辑
        IsAlive = true;
        // 重置玩家位置、生命值等
        movePlayer = playerGB.GetComponent<MovePlayer>();
        playerBody = playerGB.GetComponent<PlayerBody>();
        BattleBehavior = playerGB.GetComponent<BattleBehavior>();

        playerBody.markColorIndex.Value = markColorIndex;
    }

    // 玩家死亡逻辑
    public void Die()
    {
        IsAlive = false;
        movePlayer.moveLock.Value = true;
        BattleBehavior.fireLock.Value = true;
    }
    
}