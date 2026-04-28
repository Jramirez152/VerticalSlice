
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public GameObject overlayPanel;       // the dark panel
    public TextMeshProUGUI overlayText;   // the message text
    public float delayBeforeReload = 3f;

    void Awake()
    {
        Instance = this;
        if (overlayPanel != null)
            overlayPanel.SetActive(false);
    }

    public void OnPlayerDeath()
    {
        ShowOverlay("You Died");
        StartCoroutine(ReloadAfterDelay());
    }

    public void OnAllWavesComplete()
    {
        ShowOverlay("YOU WIN!!!!!!!!");
        StartCoroutine(ReloadAfterDelay());
    }

    void ShowOverlay(string message)
    {
        if (overlayPanel != null) overlayPanel.SetActive(true);
        if (overlayText != null) overlayText.text = message;
        Time.timeScale = 0f;
    }

    IEnumerator ReloadAfterDelay()
    {
        yield return new WaitForSecondsRealtime(delayBeforeReload);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}