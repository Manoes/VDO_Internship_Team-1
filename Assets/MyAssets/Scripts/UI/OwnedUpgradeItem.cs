using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OwnedUpgradeItem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI countText;

    private int count = 1;

    /// <summary>
    /// Initializes the UI element with the upgrade icon.
    /// </summary>
    public void Setup(Sprite icon)
    {
        if (iconImage != null) iconImage.sprite = icon;
        UpdateUI();
    }

    /// <summary>
    /// Increments the stack count and refreshes the display.
    /// </summary>
    public void Increment()
    {
        count++;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (countText != null)
        {
            countText.text = "x" + count;
            // Optionally hide the text if the count is only 1
            countText.gameObject.SetActive(count > 1);
        }
    }
}