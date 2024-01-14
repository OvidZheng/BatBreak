using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.VisualScripting;

public class HUDController : NetworkBehaviour
{
    private BattleBehavior playerBattleBehavior;
    public Image bulletsImage;
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
}