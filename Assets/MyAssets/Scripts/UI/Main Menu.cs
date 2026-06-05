using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayButton()
    {
        SceneManager.LoadScene("Main Game");
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
