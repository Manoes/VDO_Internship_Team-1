using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathUIManager : Singleton<DeathUIManager>
{
    [Header("Panels")]
    [SerializeField] private GameObject hud;
    [SerializeField] private GameObject deathUI;
    [SerializeField] private GameObject nameInputPanel;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI finalLevelText;
    [SerializeField] private TextMeshProUGUI highScoreStatusText;

    [Header("Input")]
    [SerializeField] private TMP_InputField nameInput;

    [Header("Buttons")]
    [SerializeField] private Button submitButton;
    [SerializeField] private Button mainMenuButton;

    [Header("keyboard")]
    [SerializeField] private Button firstKeyboardButton;

    [Header("Scene")]
    [SerializeField] private string mainMenuSceneName = "Main menu";

    private int finalScore;
    private int finalLevel;
    private bool isHighScore;
    private bool submitted;

    protected override void Awake()
    {
        base.Awake();

        if (deathUI != null)
            deathUI.SetActive(false);
    }

    public void ShowDeathScreen()
    {
        Time.timeScale = 0f;

        if (hud != null)
            hud.SetActive(false);

        if (deathUI != null)
            deathUI.SetActive(true);

        finalScore = Timer.Instance != null ? Timer.Instance.CurrentScore : 0;

        finalLevel = PlayerLevelSystem.Instance != null
            ? PlayerLevelSystem.Instance.CurrentLevel
            : 1;

        if (finalScoreText != null)
            finalScoreText.text = $"Score: {finalScore:D8}";

        if (finalLevelText != null)
            finalLevelText.text = $"LVL {finalLevel}";

        isHighScore = HighScoreSystem.HighScoreService.IsHighScore(finalScore, finalLevel);

        if (isHighScore)
            ShowNameInput();
        else
            ShowNoHighScore();
    }

    private void ShowNameInput()
    {
        submitted = false;

        if (highScoreStatusText != null)
            highScoreStatusText.text = "NEW HIGH SCORE";

        if (nameInputPanel != null)
            nameInputPanel.SetActive(true);

        if (nameInput != null)
            nameInput.text = "";

        StartCoroutine(SelectFirstKeyboardButton());
    }

    private IEnumerator SelectFirstKeyboardButton()
    {
        yield return null;

        EventSystem.current.SetSelectedGameObject(null);

        if (firstKeyboardButton != null)
        {
            firstKeyboardButton.Select();
            EventSystem.current.SetSelectedGameObject(firstKeyboardButton.gameObject);
        }
    }

    private void ShowNoHighScore()
    {
        if (highScoreStatusText != null)
            highScoreStatusText.text = "GAME OVER";

        if (nameInputPanel != null)
            nameInputPanel.SetActive(false);

        if (mainMenuButton != null)
            EventSystem.current.SetSelectedGameObject(mainMenuButton.gameObject);
    }

    public void SubmitHighScore()
    {
        if (submitted) return;

        submitted = true;

        if(submitButton != null)
            submitButton.interactable = false;

        if (isHighScore)
        {
            string playerName = nameInput != null ? nameInput.text : "Unknown";

            if (string.IsNullOrWhiteSpace(playerName))
                playerName = "Unknown";

            HighScoreSystem.HighScoreService.AddHighScore(
                playerName,
                finalScore,
                finalLevel
            );

            isHighScore = false;
        }

        GoToMainMenu();
    }

    public void AddCharacter(string character)
    {
        if (nameInput == null) return;
        if (string.IsNullOrEmpty(character)) return;

        nameInput.text += character;
    }

    public void DeleteCharacter()
    {
        if (nameInput == null) return;
        if (string.IsNullOrEmpty(nameInput.text)) return;

        nameInput.text = nameInput.text[..^1];
    }

    public void AddSpace()
    {
        AddCharacter(" ");
    }

    public void ClearName()
    {
        if (nameInput == null) return;

        nameInput.text = "";
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}