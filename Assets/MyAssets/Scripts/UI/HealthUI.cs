using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Health healthComponent;
    [SerializeField] private GameObject[] heartIcons;

    [Header("Animation")]
    [SerializeField] private float popScale = 1.4f;
    [SerializeField] private float popDuration = 0.15f;

    private int previousHealth;

    private void OnEnable()
    {
        if (healthComponent != null)
        {
            // Subscribe to the event that tells us when health changes
            healthComponent.OnHealthChanged.AddListener(UpdateHealthUI);

            previousHealth = healthComponent.CurrentHealth;
            
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
        if(currentHealth < previousHealth)
        {
            int lostHeartIndex = previousHealth - 1;

            if(lostHeartIndex >= 0 && lostHeartIndex < heartIcons.Length)
            {
                Transform heart = heartIcons[lostHeartIndex].transform;

                heart.DOKill();

                heart
                    .DOScale(popScale, popDuration)
                    .SetEase(Ease.OutBack)
                    .OnComplete(() =>
                    {
                        heart
                            .DOScale(0f, popDuration)
                            .SetEase(Ease.InBack)
                            .OnComplete(() =>
                            {
                                heartIcons[lostHeartIndex].SetActive(false);
                                heart.localScale = Vector3.one;
                            });
                    });
            }
        }

        for (int i = 0; i < heartIcons.Length; i++)
        {
            if(currentHealth >= previousHealth)
                heartIcons[i].SetActive(i < currentHealth);
            
        }

        previousHealth = currentHealth;
    }
}
