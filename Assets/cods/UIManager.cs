using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI levelText;
    public GameObject gameOverPanel;
    public GameObject winPanel; // 1. إضافة شاشة الفوز

    private void OnEnable()
    {
        RockBossAI.OnLevelChanged += UpdateLevelText;
    }

    private void OnDisable()
    {
        RockBossAI.OnLevelChanged -= UpdateLevelText;
    }

    void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false); // إخفاءها في البداية

        UpdateLevelText(1);
    }

    private void UpdateLevelText(int newLevel)
    {
        if (levelText != null)
        {
            levelText.text = "Level: " + newLevel.ToString();
        }
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    // 2. دالة إظهار شاشة الفوز
    public void ShowWinScreen()
    {
        if (winPanel != null) winPanel.SetActive(true);
    }
}