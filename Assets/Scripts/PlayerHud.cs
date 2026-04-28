
using UnityEngine;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [Header("References")]
    public PlayerHealth playerHealth;
    public PlayerController playerController;

    [Header("UI Elements")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI dashText;

    void Update()
    {
        if (playerHealth != null && healthText != null)
            healthText.text = $"HP: {Mathf.CeilToInt(playerHealth.currentHealth)}";

        if (playerController != null && dashText != null)
        {
            if (playerController.DashCooldownRemaining > 0f)
                dashText.text = $"DASH: {playerController.DashCooldownRemaining:F1}s";
            else
                dashText.text = "DASH: READY";
        }
    }
}