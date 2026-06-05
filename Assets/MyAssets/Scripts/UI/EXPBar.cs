using UnityEngine;
using UnityEngine.UI;

public class EXPBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;

    private void Start()
    {
        if (PlayerLevelSystem.Instance != null)
            UpdateXPBar(PlayerLevelSystem.Instance.CurrentXP, PlayerLevelSystem.Instance.XPToNextLevel);
    }

    public void UpdateXPBar(int currentXP, int maxXP)
    {
        if (fillImage != null && maxXP > 0)
            fillImage.fillAmount = (float)currentXP / maxXP;
    }
}
