using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Health healthComponent;
    [SerializeField] private GameObject[] heartIcons;

    private void OnEnable()
    {
        if (healthComponent != null)
        {
            // Subscribe to the event that tells us when health changes
            healthComponent.OnHealthChanged.AddListener(UpdateHealthUI);
            
            // Initialize the UI with the current health values
            UpdateHealthUI(healthComponent.CurrentHealth, healthComponent.MaxHealth);
        }
    }

    private void OnDisable()
    {
        if (healthComponent != null)
        {
            // Unsubscribe to prevent memory leaks
            healthComponent.OnHealthChanged.RemoveListener(UpdateHealthUI);
        }
    }

    private void UpdateHealthUI(int currentHealth, int maxHealth)
    {
        // Loop through all assigned icons
        for (int i = 0; i < heartIcons.Length; i++)
        {
            // If the current index is less than health, show the icon. 
            // Otherwise, hide it.
            heartIcons[i].SetActive(i < currentHealth);
        }
    }
}
