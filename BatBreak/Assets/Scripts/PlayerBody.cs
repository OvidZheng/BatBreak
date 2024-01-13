using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerBody : NetworkBehaviour
{
    public NetworkVariable<int> Health = new NetworkVariable<int>(100);
    private Renderer playerRenderer; // Renderer to change the color of the player

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
    

    private void UpdatePlayerColor()
    {
        // Map the health to a color value (green to red)
        float healthPercentage = Health.Value / 100f;
        Color playerColor = Color.Lerp(Color.red, Color.green, healthPercentage);


        playerRenderer.material.SetColor("_PlayerBaseColor", playerColor);

    }
}