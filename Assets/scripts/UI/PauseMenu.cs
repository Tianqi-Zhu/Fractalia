using UnityEngine;
using UnityEngine.UI; 

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false; 
    public GameObject PauseMenuUI; 
    public Button ViewTreeButton;
    public GameObject HelpMenuUI; 
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Pause();
        } 
    }

    public void Resume() 
    {
        PauseMenuUI.SetActive(false);
        GameIsPaused = false; 
        Time.timeScale = 1f; 
        ViewTreeButton.interactable = true;
    }

    void Pause() 
    {
        PauseMenuUI.SetActive(true);
        GameIsPaused = true; 
        Time.timeScale = 0f; 
        ViewTreeButton.interactable = false;
    }

    public void QuitGame() 
    {
        Debug.Log("Quit game");
        Application.Quit();
    }

    public void Help() 
    {
        HelpMenuUI.SetActive(true);
        PauseMenuUI.SetActive(false);
    }

    public void Back() 
    {
        HelpMenuUI.SetActive(false);
        PauseMenuUI.SetActive(true);
    }
}