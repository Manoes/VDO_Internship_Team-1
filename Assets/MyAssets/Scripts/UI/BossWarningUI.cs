using UnityEngine;
using TMPro;
using DG.Tweening;

using UnityEngine.UI; // Required for Image

public class BossWarningUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI warningText;
    [SerializeField] private Image iconImage; // New: Reference to the Image component for the icon

    [Header("Settings")]
    [SerializeField] private string warningMessage = "BOSS APPROACHING!";
    [SerializeField] private Sprite bossIcon; // New: The sprite to display as the boss icon
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float displayDuration = 2.0f;

    private Tween activeTween;

    private void Awake()
    {   
        if (warningText != null) {
            // Initialize hidden
            warningText.alpha = 0;
            warningText.gameObject.SetActive(false);
        }
        if (iconImage != null) { // New: Initialize icon hidden
            iconImage.color = new Color(iconImage.color.r, iconImage.color.g, iconImage.color.b, 0);
            iconImage.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Displays the boss warning with a fade-in and fade-out animation.
    /// </summary>
    public void ShowWarning()
    {
        if (warningText == null) return;
        // Icon is optional, so we don't return if it's null

        // Reset state and kill any overlapping tweens
        activeTween?.Kill();
        warningText.text = warningMessage;
        warningText.gameObject.SetActive(true);
        warningText.alpha = 0;
        
        // New: Setup and show icon if available
        if (iconImage != null)
        {
            iconImage.sprite = bossIcon;
            iconImage.gameObject.SetActive(true);
            iconImage.color = new Color(iconImage.color.r, iconImage.color.g, iconImage.color.b, 0); // Reset alpha
        }

        // Sequence: Fade In -> Wait -> Fade Out -> Disable Object
        Sequence warningSequence = DOTween.Sequence();
        warningSequence.Append(warningText.DOFade(1f, fadeDuration));
        if (iconImage != null)
        {
            warningSequence.Join(iconImage.DOFade(1f, fadeDuration)); // Fade icon in simultaneously
        }
        activeTween = warningSequence
            .AppendInterval(displayDuration)
            .Append(warningText.DOFade(0f, fadeDuration))
            .OnComplete(() => {
                warningText.gameObject.SetActive(false);
                if (iconImage != null) iconImage.gameObject.SetActive(false); // New: Disable icon
            })
            .SetUpdate(true); // Works even if game is paused/slowed
    }
}
