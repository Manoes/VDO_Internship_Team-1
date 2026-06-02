using UnityEngine;
using UnityEngine.UI;

public class EXPBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;

    private void OnEnable()
    {
        if (PlayerLevelSystem.Instance != null)
            PlayerLevelSystem.Instance.OnXPChanged.AddListener(UpdateXPBar);
    }

    private void OnDisable()
    {
        if (PlayerLevelSystem.Instance != null)
            PlayerLevelSystem.Instance.OnXPChanged.RemoveListener(UpdateXPBar);
    }

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
