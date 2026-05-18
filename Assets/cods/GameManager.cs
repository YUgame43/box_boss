using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public UIManager uiManager;
    public MonoBehaviour cameraScriptToDisable;

    [Header("Settings")]
    public float fallDeathY = -10f;
    public int winLevel = 5; // المستوى المطلوب للفوز

    private bool isGameOver = false;

    // 1. تفعيل الاستماع لتغير المستوى
    private void OnEnable()
    {
        RockBossAI.OnLevelChanged += CheckWinCondition;
    }

    private void OnDisable()
    {
        RockBossAI.OnLevelChanged -= CheckWinCondition;
    }

    void Start()
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
        if (uiManager == null) uiManager = Object.FindFirstObjectByType<UIManager>();
    }

    void Update()
    {
        if (isGameOver) return;

        if (player.position.y < fallDeathY)
        {
            TriggerGameOver();
        }
    }

    // 2. فحص حالة الفوز مع كل مستوى جديد
    private void CheckWinCondition(int currentLevel)
    {
        if (currentLevel >= winLevel && !isGameOver)
        {
            TriggerGameWon();
        }
    }

    // 3. دالة إعلان الفوز
    private void TriggerGameWon()
    {
        isGameOver = true;

        if (cameraScriptToDisable != null) cameraScriptToDisable.enabled = false;

        var playerScript = player.GetComponent<ThirdPersonController>();
        if (playerScript != null) playerScript.enabled = false;

        if (uiManager != null) uiManager.ShowWinScreen(); // تشغيل واجهة الفوز

        Time.timeScale = 0f; // إيقاف الزمن

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void TriggerGameOver()
    {
        isGameOver = true;
        player.SetParent(null);

        if (cameraScriptToDisable != null) cameraScriptToDisable.enabled = false;

        var playerScript = player.GetComponent<ThirdPersonController>();
        if (playerScript != null) playerScript.enabled = false;

        if (uiManager != null) uiManager.ShowGameOver();

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}