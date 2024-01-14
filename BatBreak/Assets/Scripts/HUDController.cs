using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.VisualScripting;

public class HUDController : NetworkBehaviour
{
    private BattleBehavior playerBattleBehavior;
    private PlayerBody _playerBody;
    public Image bulletsImage;
    public TextMeshProUGUI healthText;
    private void Start()
    {
        if ((IsServer && !IsHost))
        {
            return;
        }
    }
    
    private void FindLocalPlayer()
    {
        if (NetworkStatic.Instance.GetlocalPlayerTransform() != null)
        {
            playerBattleBehavior = NetworkStatic.Instance.GetlocalPlayerTransform().GetComponent<BattleBehavior>();
            _playerBody = NetworkStatic.Instance.GetlocalPlayerTransform().GetComponent<PlayerBody>();
        }
    }
    
    private void Update()
    {

        if ((IsServer && !IsHost))
        {
            return;
        }
        if (playerBattleBehavior != null)
        {
            UpdateBulletsUI();
            UpdateHealthUI();
        }
        else
        {
            FindLocalPlayer();
        }
    }
    
    private void UpdateBulletsUI()
    {
        if (playerBattleBehavior != null)
        {
            int currentBullets = playerBattleBehavior.GetCurrentBullets();
            float bulletsPercentage = (float)currentBullets / playerBattleBehavior.maxBullets;
            bulletsImage.fillAmount = bulletsPercentage;
        }
    }
    
    private void UpdateHealthUI()
    {
        // 显示健康值
        healthText.text = "Health: " + _playerBody.Health.Value.ToString();
    }
}