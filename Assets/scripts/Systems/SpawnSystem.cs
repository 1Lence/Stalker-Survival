using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSystem : MonoBehaviour
{
    public static SpawnSystem Instance { get; private set; }
    
    [SerializeField] private GameObject bot1;
    [SerializeField] private GameObject bot2;
    [SerializeField] private GameObject bot3;
    
    [Header("Настройки спавна")]
    [SerializeField] private int numberBot1OnScene; //число бот1 на сцене
    [SerializeField] private int numberBot2OnScene; //число бот1 на сцене
    [SerializeField] private int numberBot3OnScene; //число бот1 на сцене
    
    [SerializeField] private int spawnRateBot1; //частота спавна ед/сек бот3
    [SerializeField] private int spawnRateBot2; //частота спавна ед/сек бот3
    [SerializeField] private int spawnRateBot3; //частота спавна ед/сек бот3
    
    [SerializeField] private float spawnDistanceFromPlayer; //дистанция спавна от игрока
    [SerializeField] private float spawnDistanceFromPlayerRand; //число, на которое может случайно отклониться значение позиции при спавне от игрока
    [SerializeField] private float spawnDisranceFromPrevBot; //дистанция спавна от прошлого бота
    [SerializeField] private float spawnDistanceFromPrevBotRand; //число, на которое может случайно отклониться значение позиции при спавне от прошлого бота

    [Header("Оптимизационные настройки")]
    [SerializeField] private float spawnCheckInterval = 0.2f;
    
    // --- Переменные для отслеживания "долга" по спавну ---
    private float accumulatedSpawnTimeBot1 = 0f;
    private float accumulatedSpawnTimeBot2 = 0f;
    private float accumulatedSpawnTimeBot3 = 0f;
    
    private float lastSpawnTimeBot1 = 0f;
    private float lastSpawnTimeBot2 = 0f;
    private float lastSpawnTimeBot3 = 0f;
    private const float minSpawnInterval = 0.1f; // Минимальный интервал между проверками спавна, чтобы не перегружать CPU
    
    public event System.Action<float> OnScoreChanged; //событие передаёт очки за смерть бота

    private List<GameObject> bot1List = new List<GameObject>();
    private List<GameObject> bot2List = new List<GameObject>();
    private List<GameObject> bot3List = new List<GameObject>();
    
    private GameObject _player;
    
    void Awake()
    {
        if (Instance is null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
        
        _player = GameObject.FindWithTag("Player");

        if (_player is null)
            Debug.LogError("Player not found");
    }
    
    void Start()
    {
        // Запускаем корутину для периодической проверки спавна
        StartCoroutine(SpawnCheckCoroutine());

        System.Collections.IEnumerator SpawnCheckCoroutine()
        {
            WaitForSeconds wait = new WaitForSeconds(spawnCheckInterval);
            while (true) // Бесконечный цикл корутины
            {
                SpawnBotLogic();
                yield return wait; // Ждем установленный интервал
            }
        }
    }

    void Update()
    {
        //SpawnBotLogic();
    }

    private void SpawnBotLogic()
    {
        float currentTime = Time.time;
        float deltaTime = spawnCheckInterval; // Используем фиксированный интервал, как задано в корутине

        // --- Обновляем "долг" по спавну для Bot1 ---
        if (spawnRateBot1 > 0 && bot1List.Count < numberBot1OnScene)
        {
            accumulatedSpawnTimeBot1 += deltaTime;
            int botsToSpawn = Mathf.FloorToInt(accumulatedSpawnTimeBot1 * spawnRateBot1); // Сколько ботов "должно" родиться за накопленное время
            botsToSpawn = Mathf.Min(botsToSpawn, numberBot1OnScene - bot1List.Count); // Не превышать желаемое количество

            if (botsToSpawn > 0)
            {
                SpawnBotsOfType(bot1, botsToSpawn, bot1List);
                accumulatedSpawnTimeBot1 -= (float)botsToSpawn / spawnRateBot1; // Вычитаем время, потраченное на спавн
            }
        }
        else
        {
            // Если лимит достигнут или частота 0, сбрасываем долг, чтобы не накапливался
            accumulatedSpawnTimeBot1 = 0f;
        }

        // --- Обновляем "долг" по спавну для Bot2 ---
        if (spawnRateBot2 > 0 && bot2List.Count < numberBot2OnScene)
        {
            accumulatedSpawnTimeBot2 += deltaTime;
            int botsToSpawn = Mathf.FloorToInt(accumulatedSpawnTimeBot2 * spawnRateBot2);
            botsToSpawn = Mathf.Min(botsToSpawn, numberBot2OnScene - bot2List.Count);

            if (botsToSpawn > 0)
            {
                SpawnBotsOfType(bot2, botsToSpawn, bot2List);
                accumulatedSpawnTimeBot2 -= (float)botsToSpawn / spawnRateBot2;
            }
        }
        else
        {
            accumulatedSpawnTimeBot2 = 0f;
        }

        // --- Обновляем "долг" по спавну для Bot3 ---
        if (spawnRateBot3 > 0 && bot3List.Count < numberBot3OnScene)
        {
            accumulatedSpawnTimeBot3 += deltaTime;
            int botsToSpawn = Mathf.FloorToInt(accumulatedSpawnTimeBot3 * spawnRateBot3);
            botsToSpawn = Mathf.Min(botsToSpawn, numberBot3OnScene - bot3List.Count);

            if (botsToSpawn > 0)
            {
                SpawnBotsOfType(bot3, botsToSpawn, bot3List);
                accumulatedSpawnTimeBot3 -= (float)botsToSpawn / spawnRateBot3;
            }
        }
        else
        {
            accumulatedSpawnTimeBot3 = 0f;
        }
    }

    // --- Новый метод для спавна нескольких ботов одного типа ---
    private void SpawnBotsOfType(GameObject botPrefab, int count, List<GameObject> botList)
    {
        for (int i = 0; i < count; i++)
        {
            // Проверяем, можно ли спавнить бота (учитывая дистанцию до других ботов этого же типа)
            if (CanSpawnBot(botList, spawnDisranceFromPrevBot, spawnDistanceFromPrevBotRand))
            {
                Vector3 spawnPos = GetSpawnPosition(spawnDistanceFromPlayer, spawnDistanceFromPlayerRand);
                SpawnBot(botPrefab, spawnPos);
                //Debug.Log($"Спавн {botPrefab.name} в {spawnPos}, список: {botList.Count}/{numberBot1OnScene или 2 или 3}"); // Нужно будет адаптировать лог
            }
            else
            {
                // Если не удалось найти место для спавна, прерываем цикл спавна для этого типа
                // Это предотвращает попытки спавна 100 ботов, если место закончилось после 10.
                Debug.Log($"SpawnSystem: Не удалось найти место для спавна {botPrefab.name}, остановка цикла спавна для этого типа на этом интервале.");
                break;
            }
        }
    }
    
    private bool ShouldSpawn(float currentTime, int spawnRate, ref float lastSpawnTime)
    {
        float minInterval = 1.0f / Mathf.Max(spawnRate, 1);
        return currentTime - lastSpawnTime >= minInterval;
    }
    
    private bool CanSpawnBot(List<GameObject> botList, float baseDistance, float distanceRand)
    {
        float minRequiredDistance = baseDistance - distanceRand;
        if (minRequiredDistance <= 0) return true;
        float minRequiredDistanceSqr = minRequiredDistance * minRequiredDistance;

        int maxAttempts = 10;
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 potentialSpawnPos = GetSpawnPosition(spawnDistanceFromPlayer, spawnDistanceFromPlayerRand);

            bool isClear = true;
            foreach (GameObject existingBot in botList)
            {
                if (existingBot is not null) continue; // Опечатка была: "is not null" -> "is null" не имеет смысла, нужно "==" или "is null". Правильный вариант: "existingBot is null" или "existingBot == null". "is not null" означает "!= null", что неправильно. Исправлено на "==".

                float distanceSqr = (existingBot.transform.position - potentialSpawnPos).sqrMagnitude;
                if (distanceSqr < minRequiredDistanceSqr)
                {
                    isClear = false;
                    break;
                }
            }

            if (isClear)
            {
                return true;
            }
        }
        return false;
    }
    
    private Vector3 GetSpawnPosition(float baseDistance, float distanceRand)
    {
        float angle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float randomDistance = baseDistance + UnityEngine.Random.Range(-distanceRand, distanceRand);
        Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * randomDistance;
        return _player.transform.position + offset;
    }

    private void SpawnBot(GameObject botPrefab, Vector3 position)
    {
        GameObject botGo = Instantiate(botPrefab, position, Quaternion.identity);
        
        BotBase botObj = botGo.GetComponent<BotBase>();
        
        if (botObj is not null)
        {
            botObj.OnDeathBot += DestroyBot;
            botObj.SetPlayerTransform(_player.transform);

            if (botPrefab == bot1)
            {
                bot1List.Add(botGo);
            }
            else if (botPrefab == bot2)
            {
                bot2List.Add(botGo);
            }
            else if (botPrefab == bot3)
            {
                bot3List.Add(botGo);
            }
            else
            {
                Debug.LogWarning($"SpawnSystem: Неизвестный префаб бота {botPrefab.name}, не добавлен в список.");
            }

            //Debug.Log($"Бот {botGo.name} заспавнен, подписан на OnDeathBot, добавлен в список.");
        }
        else
        {
            Debug.LogError($"SpawnSystem: У заспавненного объекта {botPrefab.name} нет компонента BotBase!");
        }
    }

    private void DestroyBot(float score, GameObject bot)
    {
        bool removedFromList = false;
        if (bot1List.Contains(bot))
        {
            bot1List.Remove(bot);
            removedFromList = true;
            Debug.Log($"Бот {bot.name} удалён из bot1List.");
        }
        else if (bot2List.Contains(bot))
        {
            bot2List.Remove(bot);
            removedFromList = true;
            Debug.Log($"Бот {bot.name} удалён из bot2List.");
        }
        else if (bot3List.Contains(bot))
        {
            bot3List.Remove(bot);
            removedFromList = true;
            Debug.Log($"Бот {bot.name} удалён из bot3List.");
        }

        if (!removedFromList)
        {
            Debug.LogWarning($"SpawnSystem: Попытка удалить бота {bot.name} из списка, но он не был найден.");
        }

        BotBase botComponent = bot.GetComponent<BotBase>();
        if (botComponent != null)
        {
            botComponent.OnDeathBot -= DestroyBot;
        }

        Destroy(bot);

        OnScoreChanged?.Invoke(score);
        //Debug.Log($"Вызвано событие OnScoreChanged с очками: {score}");
    }

    [ContextMenu("Spawn Bot")]
    public void Spawn()
    {
        SpawnBot(bot1, GetSpawnPosition(5f, 5f));
    }
}
