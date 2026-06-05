using UnityEngine;
using TMPro;

public class Timer : Singleton<Timer>
{
    [SerializeField] private TextMeshProUGUI timerText;

    private float score = 0f;
    private bool timerRunning = true;

    public int CurrentScore => Mathf.RoundToInt(score);
    
    void Update()
    {
        if (!timerRunning) return;

        score += Time.deltaTime;
        UpdateTimerDisplay();
    }

    private void UpdateTimerDisplay()
    {
        timerText.text = (Mathf.RoundToInt(score) * 10).ToString("D8");
    }

    public void AddScore(int anmount)
    {
        score += anmount;
        UpdateTimerDisplay();
    }

    public void StopTimer() => timerRunning = false;
    public void StartTimer() => timerRunning = true;
}
