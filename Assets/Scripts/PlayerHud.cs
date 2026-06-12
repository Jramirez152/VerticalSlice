using UnityEngine;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [Header("References")]
    public PlayerHealth playerHealth;
    public PlayerController playerController;
    public EnemySpawner enemySpawner;

    [Header("UI Elements")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI dashText;
    public TextMeshProUGUI pushText;
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI enemyCountText;

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

        if (enemySpawner != null && waveText != null)
            waveText.text = $"Wave: {enemySpawner.currentWave + 1} / 3";

        if (enemySpawner != null && enemyCountText != null)
        {
            int count = GameObject.FindGameObjectsWithTag("Enemy").Length;
            enemyCountText.text = $"Enemies: {count}";
        }
    }
}