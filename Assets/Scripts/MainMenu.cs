using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Scene1"); // nom EXACT de ta scène de jeu
    }

    public void QuitGame()
    {
        Application.Quit();
        // Ne marche que dans un build, pas dans l’éditeur
        Debug.Log("QuitGame() called");
        Application.Quit();
    }
}
