using UnityEngine;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject highscoresMenu;

    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject backButton;

    public void OpenHighscores()
    {
        mainMenu.SetActive(false);
        highscoresMenu.SetActive(true);

        // Reload Highscores

        EventSystem.current.SetSelectedGameObject(backButton);
    }

    public void OpenMainMenu()
    {
        highscoresMenu.SetActive(false);
        mainMenu.SetActive(true);

        EventSystem.current.SetSelectedGameObject(playButton);
    }
}