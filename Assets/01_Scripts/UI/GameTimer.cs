using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] private bool startAutomatically = true;
    [SerializeField] private bool countUp = true; // true = count up, false = count down
    
    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI timerText;
    
    [Header("Debug Info")]
    [SerializeField] private float totalSeconds = 0f;
    [SerializeField] private bool isRunning = false;
    
    // Public properties to access timer values
    public float TotalSeconds => totalSeconds;
    public int Minutes => Mathf.FloorToInt(totalSeconds / 60f);
    public int Seconds => Mathf.FloorToInt(totalSeconds % 60f);
    public bool IsRunning => isRunning;
    
    // Events for timer milestones
    public System.Action<float> OnTimerUpdate;
    public System.Action OnTimerStart;
    public System.Action OnTimerStop;
    public System.Action OnTimerReset;

    private void Start()
    {
        // Try to get TextMeshProUGUI component if not assigned
        if (timerText == null)
        {
            timerText = GetComponent<TextMeshProUGUI>();
            if (timerText == null)
            {
                timerText = GetComponentInChildren<TextMeshProUGUI>();
            }
        }
        
        if (startAutomatically)
        {
            StartTimer();
        }
        
        UpdateTimerDisplay();
    }

    private void Update()
    {
        if (isRunning)
        {
            if (countUp)
            {
                totalSeconds += Time.deltaTime;
            }
            else
            {
                totalSeconds -= Time.deltaTime;
                
                // Stop countdown at zero
                if (totalSeconds <= 0f)
                {
                    totalSeconds = 0f;
                    StopTimer();
                }
            }
            
            UpdateTimerDisplay();
            OnTimerUpdate?.Invoke(totalSeconds);
        }
    }

    /// <summary>
    /// Start the timer
    /// </summary>
    public void StartTimer()
    {
        isRunning = true;
        OnTimerStart?.Invoke();
        Debug.Log("Timer started");
    }

    /// <summary>
    /// Stop the timer
    /// </summary>
    public void StopTimer()
    {
        isRunning = false;
        OnTimerStop?.Invoke();
        Debug.Log($"Timer stopped at {GetFormattedTime()}");
    }

    /// <summary>
    /// Pause the timer (same as stop)
    /// </summary>
    public void PauseTimer()
    {
        StopTimer();
    }

    /// <summary>
    /// Resume the timer (same as start)
    /// </summary>
    public void ResumeTimer()
    {
        StartTimer();
    }

    /// <summary>
    /// Reset the timer to zero
    /// </summary>
    public void ResetTimer()
    {
        totalSeconds = 0f;
        isRunning = false;
        UpdateTimerDisplay();
        OnTimerReset?.Invoke();
        Debug.Log("Timer reset");
    }

    /// <summary>
    /// Set timer to specific time in seconds
    /// </summary>
    public void SetTime(float seconds)
    {
        totalSeconds = Mathf.Max(0f, seconds);
        UpdateTimerDisplay();
    }

    /// <summary>
    /// Set timer to specific minutes and seconds
    /// </summary>
    public void SetTime(int minutes, int seconds)
    {
        totalSeconds = (minutes * 60f) + seconds;
        UpdateTimerDisplay();
    }

    /// <summary>
    /// Add time to the current timer
    /// </summary>
    public void AddTime(float seconds)
    {
        totalSeconds += seconds;
        if (totalSeconds < 0f) totalSeconds = 0f;
        UpdateTimerDisplay();
    }

    /// <summary>
    /// Get formatted time string (MM:SS)
    /// </summary>
    public string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(totalSeconds / 60f);
        int seconds = Mathf.FloorToInt(totalSeconds % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    /// <summary>
    /// Get formatted time string with milliseconds (MM:SS.mmm)
    /// </summary>
    public string GetFormattedTimeWithMilliseconds()
    {
        int minutes = Mathf.FloorToInt(totalSeconds / 60f);
        int seconds = Mathf.FloorToInt(totalSeconds % 60f);
        int milliseconds = Mathf.FloorToInt((totalSeconds * 1000f) % 1000f);
        return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
    }

    /// <summary>
    /// Update the timer display text
    /// </summary>
    private void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            timerText.text = GetFormattedTime();
        }
    }

    /// <summary>
    /// Toggle timer between running and stopped
    /// </summary>
    public void ToggleTimer()
    {
        if (isRunning)
        {
            StopTimer();
        }
        else
        {
            StartTimer();
        }
    }
}