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
    public TextMeshProUGUI pushText;

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

        if (playerController != null && pushText != null)
        {
            if (playerController.PushCooldownRemaining > 0f)
                pushText.text = $"PUSH: {playerController.PushCooldownRemaining:F1}s";
            else
                pushText.text = "PUSH: READY";
        }
    }
    [Header("Wave UI")]
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI enemyCountText;

    public void UpdateWaveUI(int wave, int enemyCount)
    {
        if (waveText != null)
            waveText.text = $"Wave: {wave} / 3";
        if (enemyCountText != null)
            enemyCountText.text = $"Enemies: {enemyCount}";
    }
}