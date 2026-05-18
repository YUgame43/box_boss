using UnityEngine;
using UnityEngine.SceneManagement; // مهم جداً للتعامل مع المشاهد

public class MainMenu : MonoBehaviour
{
    // دالة الانتقال للعبة
    public void StartGame()
    {
        // رقم 1 هو الـ Index الخاص بمشهد اللعبة
        SceneManager.LoadScene(1);
    }

    // دالة إضافية ممتازة لزر الخروج من اللعبة
    public void QuitGame()
    {
        Debug.Log("Game Quit!"); // بتظهر جوا Unity
        Application.Quit(); // بتشتغل بس تعمل Build
    }
}