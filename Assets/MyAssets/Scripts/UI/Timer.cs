using UnityEngine;
using TMPro;
using Unity.Mathematics;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;

    private float score = 0f;
    private bool timerRunning = true;
    
    void Update()
    {
        if (!timerRunning) return;

        score += Time.deltaTime;
        UpdateTimerDisplay();
    }

    private void UpdateTimerDisplay()
    {
        int currentScore = Mathf.RoundToInt(score);
        timerText.text = $"{Mathf.RoundToInt(currentScore * 10f)}";
    }

    public void AddScore(int anmount)
    {
        score += anmount;
        UpdateTimerDisplay();
    }

    public void StopTimer() => timerRunning = false;
    public void StartTimer() => timerRunning = true;
}
