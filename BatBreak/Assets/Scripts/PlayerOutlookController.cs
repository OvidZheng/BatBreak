using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerOutlookController : NetworkBehaviour
{
    public NetworkVariable<int> markColorIndex = new NetworkVariable<int>(0);
    public List<Renderer> playerMarkRenderers;
    public List<Renderer> playerHealthRenders;
    private PlayerBody playerBody;
    private bool isOccupied = false;

    private Color[] playerColors = new Color[] {
        new Color(0, 0, 1, 1), // 亮蓝色
        new Color(1, 1, 0, 1), // 亮黄色
        new Color(1, 0, 1, 1), // 亮紫色
        new Color(0, 1, 1, 1), // 亮青色
        new Color(1, 0.5f, 0, 1), // 亮橙色
        new Color(1, 0.5f, 0.5f, 1), // 亮粉色
        new Color(0.5f, 0.5f, 1, 1), // 亮天蓝色
        new Color(0.5f, 1, 0.5f, 1), // 亮薄荷绿
        new Color(1, 0.75f, 0.5f, 1) // 亮桃色
    };

    private void Start()
    {
        playerBody = GetComponent<PlayerBody>();
        isOccupied = false;
    }

    private void Update()
    {
        if (IsClient)
        {
            UpdatePlayerColor();
        }
    }

    private void UpdatePlayerColor()
    {
        if (isOccupied)
        {
            return;
        }
        float healthPercentage = playerBody.Health.Value / 100f;
        Color playerHealthColor = Color.Lerp(Color.red, Color.green, healthPercentage);

        foreach (Renderer r in playerHealthRenders)
        {
            r.material.SetColor("_PlayerBaseColor", playerHealthColor);
        }

        Color playerMarkColor = playerColors[markColorIndex.Value];
        foreach (Renderer markRenderer in playerMarkRenderers)
        {
            markRenderer.material.SetColor("_PlayerBaseColor", playerMarkColor);
        }
    }

    [ServerRpc]
    public void RequestChangeMarkColorServerRpc(ServerRpcParams rpcParams = default)
    {
        markColorIndex.Value = (markColorIndex.Value + 1) % playerColors.Length;
    }
    
    
    [ServerRpc]
    public void RequestChangeMarkColorSpecificServerRpc(int colorIndex, ServerRpcParams rpcParams = default)
    {
        markColorIndex.Value = colorIndex;
    }

    
    
    private void OnEnable()
    {
        NetworkManagerUI.OnColorChangeRequested += HandleColorChange;
    }

    private void OnDisable()
    {
        NetworkManagerUI.OnColorChangeRequested -= HandleColorChange;
    }
    
    private void HandleColorChange()
    {
        if (IsOwner) // 确保只有当前客户端的玩家执行这个操作
        {
            RequestChangeColor();
        }
    }

    public void RequestChangeColor()
    {
        if (IsClient)
        {
            // 请求服务器更改颜色
            RequestChangeMarkColorServerRpc();
        }
    }
    
    private IEnumerator FlashPlayerHealthRenderers(float duration, int flashCount)
    {
        isOccupied = true;
        Color originalColor = Color.Lerp(Color.red, Color.green, playerBody.Health.Value / 100f);
        Color flashColor = Color.white;

        float flashDuration = duration / (flashCount * 2);
        
        for (int i = 0; i < flashCount; i++)
        {
            // 将颜色设置为白色
            SetPlayerHealthRenderersColor(flashColor);
            yield return new WaitForSeconds(flashDuration);

            // 恢复原始颜色
            SetPlayerHealthRenderersColor(originalColor);
            yield return new WaitForSeconds(flashDuration);
        }
        isOccupied = false;
    }

    private void SetPlayerHealthRenderersColor(Color color)
    {
        foreach (Renderer renderer in playerHealthRenders)
        {
            renderer.material.SetColor("_PlayerBaseColor", color);
        }
    }

    public void PlayerHealthRendererFlash(float duration, int flashCount)
    {
        if (isOccupied || !IsClient)
        {
                return;
        }
        StartCoroutine(FlashPlayerHealthRenderers(duration, flashCount));
    }
}