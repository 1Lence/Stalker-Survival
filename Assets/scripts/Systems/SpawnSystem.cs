using System;
using UnityEngine;

public class SpawnSystem : MonoBehaviour
{
    public static SpawnSystem Instance { get; private set; }
    
    [Header("Префабы ботов")]
    [SerializeField] private GameObject bot1;
    [SerializeField] private GameObject bot2;
    [SerializeField] private GameObject bot3;
    
    [Header("Настройки спавна")]
    [SerializeField] private int numberBot1OnScene; //желаемое число бот1 на сцене
    [SerializeField] private int numberBot2OnScene; //желаемое число бот2 на сцене
    [SerializeField] private int numberBot3OnScene; //желаемое число бот3 на сцене
    
    [SerializeField] private int spawnRateBot1; //желаемая частота спавна ед/сек бот1
    [SerializeField] private int spawnRateBot2; //желаемая частота спавна ед/сек бот2
    [SerializeField] private int spawnRateBot3; //желаемая частота спавна ед/сек бот3
    
    [SerializeField] private float spawnDistanceFromPlayer; //базовая дистанция спавна от игрока
    [SerializeField] private float spawnDistanceFromPlayerRand; //допустимое отклонение от базовой дистанции спавна от игрока
    // spawnDisranceFromPrevBot и spawnDistanceFromPrevBotRand убираем, так как не используем проверку расстояния до других ботов

    // --- Переменные для отслеживания времени последнего спавна ---
    private float lastSpawnTimeBot1 = 0f;
    private float lastSpawnTimeBot2 = 0f;
    private float lastSpawnTimeBot3 = 0f;
    
    public event System.Action<float> OnScoreChanged; //событие передаёт очки за смерть бота

    // --- Переменные для подсчёта количества ботов на сцене ---
    private int Bot1Number = 0;
    private int Bot2Number = 0;
    private int Bot3Number = 0;
    
    private GameObject _player;
    private PlayerControl _playerControl;
    
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
        if (_player != null)
        {
             _playerControl = _player.GetComponent<PlayerControl>();
        }
        else
        {
             Debug.LogError("Player not found");
        }
    }

    void Update()
    {
        // Вызываем логику спавна каждый кадр
        SpawnBotLogic();
        // Debug.Log(Bot1Number + Bot2Number + Bot3Number); // Убираем, если не нужно постоянно выводить
    }

    private void SpawnBotLogic()
    {
        float currentTime = Time.time;

        // --- Проверяем и спавним Bot1 ---
        if (spawnRateBot1 > 0 && Bot1Number < numberBot1OnScene)
        {
            float minSpawnInterval = 1.0f / spawnRateBot1;
            if (currentTime - lastSpawnTimeBot1 >= minSpawnInterval)
            {
                // Упрощённая проверка CanSpawnBot - только расстояние до игрока
                if (CanSpawnBotBasicCheck())
                {
                    Vector3 spawnPos = GetSpawnPosition(spawnDistanceFromPlayer, spawnDistanceFromPlayerRand);
                    SpawnBot(bot1, spawnPos);
                    lastSpawnTimeBot1 = currentTime; // Обновляем время последнего спавна
                }
            }
        }

        // --- Проверяем и спавним Bot2 ---
        if (spawnRateBot2 > 0 && Bot2Number < numberBot2OnScene)
        {
            float minSpawnInterval = 1.0f / spawnRateBot2;
            if (currentTime - lastSpawnTimeBot2 >= minSpawnInterval)
            {
                if (CanSpawnBotBasicCheck())
                {
                    Vector3 spawnPos = GetSpawnPosition(spawnDistanceFromPlayer, spawnDistanceFromPlayerRand);
                    SpawnBot(bot2, spawnPos);
                    lastSpawnTimeBot2 = currentTime;
                }
            }
        }

        // --- Проверяем и спавним Bot3 ---
        if (spawnRateBot3 > 0 && Bot3Number < numberBot3OnScene)
        {
            float minSpawnInterval = 1.0f / spawnRateBot3;
            if (currentTime - lastSpawnTimeBot3 >= minSpawnInterval)
            {
                if (CanSpawnBotBasicCheck())
                {
                    Vector3 spawnPos = GetSpawnPosition(spawnDistanceFromPlayer, spawnDistanceFromPlayerRand);
                    SpawnBot(bot3, spawnPos);
                    lastSpawnTimeBot3 = currentTime;
                }
            }
        }
    }

    // --- Упрощённая проверка возможности спавна ---
    private bool CanSpawnBotBasicCheck()
    {
        // Проверка, что минимальное расстояние от игрока соблюдено
        Vector3 potentialSpawnPos = GetSpawnPosition(spawnDistanceFromPlayer, spawnDistanceFromPlayerRand);
        float distanceToPlayer = Vector3.Distance(potentialSpawnPos, _player.transform.position);
        float minDistanceToPlayer = spawnDistanceFromPlayer - spawnDistanceFromPlayerRand;

        if (distanceToPlayer < minDistanceToPlayer)
        {
            return false; // Позиция слишком близко к игроку
        }

        // Можно добавить другие базовые проверки (например, не в стене ли)
        // RaycastHit2D hit = Physics2D.Raycast(potentialSpawnPos, Vector2.zero, 0.1f);
        // if (hit.collider != null) return false; // Пример проверки на коллизию

        return true; // Позиция валидна
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
        
        if (botObj != null)
        {
            // --- Подписка на событие смерти ---
            botObj.OnDeathBot += DestroyBot; 

            botObj.SetPlayerTransform(_player.transform);
            if (_playerControl != null)
            {
                 botObj.SetPlayerControl(_playerControl);
            }

            // --- Увеличиваем счётчик ботов соответствующего типа ---
            if (botObj.BotId == 1)
            {
                Bot1Number++;
            }
            else if (botObj.BotId == 2)
            {
                Bot2Number++;
            }
            else if (botObj.BotId == 3)
            {
                Bot3Number++;
            }
            else
            {
                Debug.LogWarning($"SpawnSystem: У бота {botGo.name} неизвестный BotId {botObj.BotId}, не увеличено количество.");
            }

            Debug.Log($"SpawnBot: Заспавнен бот с BotId {botObj.BotId}. Счётчики: 1-{Bot1Number}, 2-{Bot2Number}, 3-{Bot3Number}");
        }
        else
        {
            Debug.LogError($"SpawnSystem: У заспавненного объекта {botPrefab.name} нет компонента BotBase!");
        }
    }

    private void DestroyBot(float score, GameObject bot)
    {
        Debug.Log($"DestroyBot вызван для {bot.name}"); // <--- Добавить

        // Уменьшаем счётчик ботов соответствующего типа
        BotBase botComponent = bot.GetComponent<BotBase>();
        if (botComponent != null)
        {
            // --- Отписка от события смерти ---
            botComponent.OnDeathBot -= DestroyBot; 

            // --- Используем BotId из компонента бота ---
            if (botComponent.BotId == 1)
            {
                Bot1Number = Mathf.Max(0, Bot1Number - 1);
                Debug.Log($"DestroyBot: Убит Bot1. Счётчик: {Bot1Number}/{numberBot1OnScene}"); // <--- Добавить
            }
            else if (botComponent.BotId == 2)
            {
                Bot2Number = Mathf.Max(0, Bot2Number - 1);
                Debug.Log($"DestroyBot: Убит Bot2. Счётчик: {Bot2Number}/{numberBot2OnScene}"); // <--- Добавить
            }
            else if (botComponent.BotId == 3)
            {
                Bot3Number = Mathf.Max(0, Bot3Number - 1);
                Debug.Log($"DestroyBot: Убит Bot3. Счётчик: {Bot3Number}/{numberBot3OnScene}"); // <--- Добавить
            }
            else
            {
                Debug.LogWarning($"SpawnSystem: У бота {bot.name} неизвестный BotId {botComponent.BotId}, не уменьшено количество.");
            }
        }
        else
        {
            Debug.LogWarning($"SpawnSystem: У бота {bot.name} не найден компонент BotBase перед отпиской.");
        }

        // Уничтожаем GameObject бота в сцене
        Destroy(bot);

        // Вызываем событие OnScoreChanged, передав ему количество очков
        OnScoreChanged?.Invoke(score);
    }

    [ContextMenu("Spawn Bot1")]
    public void Spawn()
    {
        SpawnBot(bot1, GetSpawnPosition(5f, 5f));
    }
}