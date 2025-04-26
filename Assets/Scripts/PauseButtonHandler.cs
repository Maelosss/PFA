using UnityEngine;

public class PauseButtonHandler : MonoBehaviour
{
    public PauseMenuManager pauseMenuManager;

    public void OnPauseButtonClicked()
    {
        pauseMenuManager.TogglePause(); 

    }
}
