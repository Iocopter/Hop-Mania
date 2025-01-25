using UnityEngine;
using UnityEngine.SceneManagement;

public class Finish : MonoBehaviour
{
    public void MenuPrincipal()
    {
        // โหลดหน้า MainMenu โดยตรง
        SceneManager.LoadScene("Main Menu");
    }

    public void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}

