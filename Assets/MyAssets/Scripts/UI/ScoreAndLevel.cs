using UnityEngine;
using TMPro;

public class ScoreAndLevel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI finalLevelText;
    private int score;
    private int level;

    /// <summary>
    /// Automatically updates the stats whenever this panel is activated.
    /// </summary>
    private void OnEnable()
    {
        UpdateDeathPanelStats();
    }

    /// <summary>
    /// Updates the displayed score and level on the death panel.
    /// This method should be called when the death panel becomes active.
    /// </summary>
    public void UpdateDeathPanelStats()
    {
        if (finalScoreText == null || finalLevelText == null)
        {
            Debug.LogWarning("Score or Level TextMeshProUGUI references are not set in ScoreAndLevel script.");
            return;
        }

        if (Timer.Instance != null)
            score = (Timer.Instance.CurrentScore * 10);
        
        if (PlayerLevelSystem.Instance != null)
            level = PlayerLevelSystem.Instance.CurrentLevel;

        UpdateDeathU(score, level);
        // Show Keypad -> name input
        // On Submit text -> HighScoreService.AddHighScore
    }

    private void UpdateDeathU(int score, int level)
    {
        if(finalLevelText != null)
            finalLevelText.text = "Max Level: " + level.ToString();

        if(finalScoreText != null)
            finalScoreText.text = "Score: " + (score).ToString("D8");
    }
}
