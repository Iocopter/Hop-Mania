using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button restartButton;      // ปุ่มสำหรับเริ่มเกมใหม่
    [SerializeField] private Button mainMenuButton;     // ปุ่มสำหรับกลับไปหน้าหลัก
    [SerializeField] private GameObject gameOverPanel;  // Panel สำหรับ Game Over

    private void Start()
    {
        // ตั้งค่าปุ่มให้ทำงานเมื่อคลิก
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);
        
        gameOverPanel.SetActive(false); // ซ่อน Panel ตั้งแต่แรก
    }

    // ฟังก์ชันสำหรับแสดงหน้าจอ Game Over
    public void ShowGameOver()
    {
        Debug.Log("Game Over Panel is activated.");
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true); // แสดง Game Over Panel
        }
        Time.timeScale = 0; // หยุดเวลา
    }

    public void RestartGame()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false); // ซ่อน Game Over Panel

        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.ResetPlayerState(); // เรียกฟังก์ชันรีเซ็ตค่าผู้เล่น
        }
        else
        {
            Debug.LogWarning("Player object not found!");
        }

        Time.timeScale = 1; // รีเซ็ตเวลาให้เกมดำเนินต่อ
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // โหลดฉากปัจจุบันใหม่
    }

    // ฟังก์ชันสำหรับกลับไปหน้าหลัก
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1; // รีเซ็ตเวลาให้เกมดำเนินต่อก่อนโหลดเมนูหลัก
        ResetGameState();  // รีเซ็ตสถานะของเกม

        SceneManager.LoadScene("Main Menu"); // เปลี่ยนเป็นชื่อที่คุณใช้ในเมนูหลัก
    }

    // ฟังก์ชันในการรีเซ็ตสถานะเกมเมื่อกลับไปที่เมนูหลัก
    private void ResetGameState()
    {
        // รีเซ็ตค่าผู้เล่นหรือสถานะที่เกี่ยวข้องกับเกมที่แพ้
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.ResetPlayerState(); // รีเซ็ตสถานะของผู้เล่น
        }

        // สามารถเพิ่มการรีเซ็ตตัวแปรหรือสถานะอื่น ๆ ที่จำเป็นที่อาจมี
        Debug.Log("Game state has been reset.");
    }

    // ฟังก์ชันสำหรับการ trigger เมื่อผู้เล่นแพ้
    public void TriggerGameOver()
    {
        Debug.Log("Player is dead! Game Over triggered.");
        GameOverController gameOverController = FindObjectOfType<GameOverController>();
        if (gameOverController != null)
        {
            gameOverController.ShowGameOver(); // เรียกฟังก์ชันที่แสดงหน้าจอ "Game Over"
        }
        else
        {
            Debug.LogError("GameOverController not found!");
        }
    }
}
