using UnityEngine;
using UnityEngine.SceneManagement;

public class GameWinController : MonoBehaviour
{
    [SerializeField] private GameObject youWinPanel;

    private void Start()
    {
        if (youWinPanel != null)
            youWinPanel.SetActive(false); // ซ่อน Panel เริ่มต้น
    }

    public void ShowYouWin()
    {
        if (youWinPanel != null)
        {
            youWinPanel.SetActive(true); // แสดง Panel You Win
            Time.timeScale = 0; // หยุดเวลาในเกม
        }
    }

public void LoadNextScene()
{
    Time.timeScale = 1; // รีเซ็ตเวลา

    int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

    // ตรวจสอบว่ามี Scene ถัดไปใน Build Settings หรือไม่
    if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
    {
        SceneManager.LoadScene(nextSceneIndex); // โหลด Scene ถัดไป
    }
    else
    {
        Debug.Log("Last level completed! Returning to the main menu.");
        SceneManager.LoadScene("MenuPrincipal"); // โหลดหน้าเมนูหลัก
    }
}


    public void BackToMenu()
    {
        // รีเซ็ตเวลาเมื่อกลับสู่เมนู
        Time.timeScale = 1;
        SceneManager.LoadScene("MenuPrincipal"); // โหลดหน้าเมนูหลัก
    }

    private void TriggerYouWin()
    {
        Debug.Log("You win!");  // ปรากฏใน Console

        // เรียกการแสดงผลหน้า You Win
        GameWinController gameWinController = FindObjectOfType<GameWinController>();
        if (gameWinController != null)
        {
            gameWinController.ShowYouWin(); // เรียกฟังก์ชันแสดงหน้าจอ "You Win"
        }
        else
        {
            Debug.LogError("GameWinController not found!");
        }
    }
}
