using System;
using UnityEngine;

public class ScoreCounter : MonoBehaviour
{
    [SerializeField] private int score; // количество очков, набранных игроком
    [SerializeField] private int surviveTime; // время выживания в секундах (только для отображения, не для расчёта)

    public System.Action<int, int> OnGameFinishedScoreAndTime;
    public System.Action<int> OnTimeChanged; // вызывается раз в секунду
    public System.Action<int> OnScoreChanged; // передаёт актуальное кол-во очков

    private float elapsedGameTime = 0f; // фактическое прошедшее время (в секундах, float)
    private bool isGameRunning = false;
    private float lastSecondUpdate = 0f; // для отслеживания целых секунд

    public void Awake()
    {
        
    }

    void Update()
    {
        if (!isGameRunning) return;

        // Наращиваем общее время игры
        elapsedGameTime += Time.deltaTime;

        // Отправляем событие каждую полную секунду
        if (Mathf.Floor(elapsedGameTime) > lastSecondUpdate)
        {
            lastSecondUpdate = Mathf.Floor(elapsedGameTime);
            surviveTime = (int)lastSecondUpdate;
            OnTimeChanged?.Invoke(surviveTime);
        }
    }

    //обновляет количество очков
    public void UpdateScore(int plusScore)
    {
        if (plusScore < 0)
        {
            Debug.LogWarning("ScoreCounter: попытка добавить отрицательные очки.");
            return;
        }
        score += plusScore;
        OnScoreChanged?.Invoke(score);
    }
    
    public void GameFinished()
    {
        if (!isGameRunning) return;

        isGameRunning = false;
        surviveTime = (int)elapsedGameTime; // финальное время
        OnGameFinishedScoreAndTime?.Invoke(score, surviveTime);
    }

    public void GameStart()
    {
        score = 0;
        elapsedGameTime = 0f;
        surviveTime = 0;
        lastSecondUpdate = -1f; // чтобы первая секунда (0→1) сработала корректно
        isGameRunning = true;

        // Сброс событий (опционально)
        OnScoreChanged?.Invoke(0);
        OnTimeChanged?.Invoke(0);
    }
}
