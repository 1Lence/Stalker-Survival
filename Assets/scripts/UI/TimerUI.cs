using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    private TextMeshProUGUI _tmp;

    void Awake()
    {
        _tmp = GetComponent<TextMeshProUGUI>();
        if (_tmp == null)
        {
            Debug.LogError("TimerUI: TextMeshPro component not found!");
        }
    }

    /// <summary>
    /// Отображает время в формате ММ:СС на основе переданных секунд.
    /// </summary>
    /// <param name="totalSeconds">Общее количество секунд</param>
    public void SetTime(int totalSeconds)
    {
        if (_tmp == null) return;

        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        // Формат: "02:05", "10:30" и т.д.
        _tmp.text = $"{minutes:D2}:{seconds:D2}";
    }

    // Опционально: перегрузка для float (если передаёшь из Time.time)
    public void SetTime(float totalSeconds)
    {
        SetTime(Mathf.FloorToInt(totalSeconds));
    }
}