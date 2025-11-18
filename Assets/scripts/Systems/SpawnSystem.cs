using System;
using UnityEngine;

public class SpawnSystem : MonoBehaviour
{
    [SerializeField] private SpawnSystemDataSO spawnSystemDataSO;
    
    [Header("Префабы ботов")]
    [SerializeField] private GameObject bot1;
    [SerializeField] private GameObject bot2;
    [SerializeField] private GameObject bot3;
    
    [Header("Настройки спавна")]
    [SerializeField] private int numberBot1OnScene;
    [SerializeField] private int numberBot2OnScene;
    [SerializeField] private int numberBot3OnScene;
    
    [SerializeField] private float spawnRateBot1 = 1f; // ботов в секунду (может быть 0.1, 0.5, 2.3 и т.д.)
    [SerializeField] private float spawnRateBot2 = 1f;
    [SerializeField] private float spawnRateBot3 = 1f;
    
    [SerializeField] private float spawnDistanceFromPlayer = 10f;
    [SerializeField] private float spawnDistanceFromPlayerRand = 2f;
    
    [Header("Настройки роста сложности")]
    [SerializeField] private float difficultyIncreaseInterval = 10f;
    [SerializeField] private int botCountIncrement = 1;
    [SerializeField] private float spawnRateIncrement = 0.2f;

    [Header("Максимумы (ограничения сложности)")]
    [SerializeField] private int maxBot1Count = 5;
    [SerializeField] private int maxBot2Count = 5;
    [SerializeField] private int maxBot3Count = 5;
    
    [SerializeField] private float maxSpawnRateBot1 = 5f;
    [SerializeField] private float maxSpawnRateBot2 = 5f;
    [SerializeField] private float maxSpawnRateBot3 = 5f;
    
    // Внутренние переменные
    private float lastSpawnTimeBot1 = 0f;
    private float lastSpawnTimeBot2 = 0f;
    private float lastSpawnTimeBot3 = 0f;
    
    private int Bot1Number = 0;
    private int Bot2Number = 0;
    private int Bot3Number = 0;
    
    private bool _isSpawningEnabled = true;
    
    private GameObject _player;
    private PlayerControl _playerControl;
    
    public event Action<int> OnScoreChanged;
    
    void Start()
    {
        bot1 = spawnSystemDataSO.bot1;
        bot2 = spawnSystemDataSO.bot2;
        bot3 = spawnSystemDataSO.bot3;
        
        numberBot1OnScene = spawnSystemDataSO.numberBot1OnScene;
        numberBot2OnScene = spawnSystemDataSO.numberBot2OnScene;
        numberBot3OnScene = spawnSystemDataSO.numberBot3OnScene;
        
        spawnRateBot1 = spawnSystemDataSO.spawnRateBot1;
        spawnRateBot2 = spawnSystemDataSO.spawnRateBot2;
        spawnRateBot3 = spawnSystemDataSO.spawnRateBot3;
        
        spawnDistanceFromPlayer = spawnSystemDataSO.spawnDistanceFromPlayer;
        spawnDistanceFromPlayerRand = spawnSystemDataSO.spawnDistanceFromPlayerRand;
        
        difficultyIncreaseInterval = spawnSystemDataSO.difficultyIncreaseInterval;
        botCountIncrement = spawnSystemDataSO.botCountIncrement;
        spawnRateIncrement = spawnSystemDataSO.spawnRateIncrement;
        
        maxBot1Count = spawnSystemDataSO.maxBot1Count;
        maxBot2Count = spawnSystemDataSO.maxBot2Count;
        maxBot3Count = spawnSystemDataSO.maxBot3Count;
        
        maxSpawnRateBot1 = spawnSystemDataSO.maxSpawnRateBot1;
        maxSpawnRateBot2 = spawnSystemDataSO.maxSpawnRateBot2;
        maxSpawnRateBot3 = spawnSystemDataSO.maxSpawnRateBot3;
        
        _player = GameObject.FindWithTag("Player");
        if (_player != null)
        {
            _playerControl = _player.GetComponent<PlayerControl>();
        }
        else
        {
            Debug.LogError("Player not found");
        }
        InvokeRepeating(nameof(IncreaseDifficulty), difficultyIncreaseInterval, difficultyIncreaseInterval);
    }

    void Update()
    {
        SpawnBotLogic();
    }

    private void SpawnBotLogic()
    {
        if (!_isSpawningEnabled) return;
        
        float currentTime = Time.time;

        // Bot1
        if (spawnRateBot1 > 0 && Bot1Number < numberBot1OnScene)
        {
            float minSpawnInterval = 1f / spawnRateBot1;
            if (currentTime - lastSpawnTimeBot1 >= minSpawnInterval && CanSpawnBotBasicCheck())
            {
                SpawnBot(bot1, GetSpawnPosition(spawnDistanceFromPlayer, spawnDistanceFromPlayerRand));
                lastSpawnTimeBot1 = currentTime;
            }
        }

        // Bot2
        if (spawnRateBot2 > 0 && Bot2Number < numberBot2OnScene)
        {
            float minSpawnInterval = 1f / spawnRateBot2;
            if (currentTime - lastSpawnTimeBot2 >= minSpawnInterval && CanSpawnBotBasicCheck())
            {
                SpawnBot(bot2, GetSpawnPosition(spawnDistanceFromPlayer, spawnDistanceFromPlayerRand));
                lastSpawnTimeBot2 = currentTime;
            }
        }

        // Bot3
        if (spawnRateBot3 > 0 && Bot3Number < numberBot3OnScene)
        {
            float minSpawnInterval = 1f / spawnRateBot3;
            if (currentTime - lastSpawnTimeBot3 >= minSpawnInterval && CanSpawnBotBasicCheck())
            {
                SpawnBot(bot3, GetSpawnPosition(spawnDistanceFromPlayer, spawnDistanceFromPlayerRand));
                lastSpawnTimeBot3 = currentTime;
            }
        }
    }

    private bool CanSpawnBotBasicCheck()
    {
        if (_player == null) return false;
        
        Vector3 potentialSpawnPos = GetSpawnPosition(spawnDistanceFromPlayer, spawnDistanceFromPlayerRand);
        float minAllowedDistance = spawnDistanceFromPlayer - spawnDistanceFromPlayerRand;
        return Vector3.Distance(potentialSpawnPos, _player.transform.position) >= minAllowedDistance;
    }
    
    private Vector3 GetSpawnPosition(float baseDistance, float distanceRand)
    {
        float angle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float randomDistance = baseDistance + UnityEngine.Random.Range(-distanceRand, distanceRand);
        return _player.transform.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * randomDistance;
    }

    private void SpawnBot(GameObject botPrefab, Vector3 position)
    {
        if (botPrefab == null) return;
        
        GameObject botGo = Instantiate(botPrefab, position, Quaternion.identity);
        BotBase botObj = botGo.GetComponent<BotBase>();
        
        if (botObj != null)
        {
            botObj.OnDeathBot += DestroyBot;
            botObj.SetPlayerTransform(_player.transform);
            if (_playerControl != null)
                botObj.SetPlayerControl(_playerControl);

            switch (botObj.BotId)
            {
                case 1: Bot1Number++; break;
                case 2: Bot2Number++; break;
                case 3: Bot3Number++; break;
                default: Debug.LogWarning($"Неизвестный BotId: {botObj.BotId}"); break;
            }
        }
        else
        {
            Debug.LogError($"У {botPrefab.name} нет BotBase!");
            Destroy(botGo);
        }
    }

    private void DestroyBot(int score, GameObject bot)
    {
        BotBase botComponent = bot.GetComponent<BotBase>();
        if (botComponent != null)
        {
            botComponent.OnDeathBot -= DestroyBot;
            switch (botComponent.BotId)
            {
                case 1: Bot1Number = Mathf.Max(0, Bot1Number - 1); break;
                case 2: Bot2Number = Mathf.Max(0, Bot2Number - 1); break;
                case 3: Bot3Number = Mathf.Max(0, Bot3Number - 1); break;
                default: Debug.LogWarning($"Неизвестный BotId у убитого бота: {botComponent.BotId}"); break;
            }
        }
        Destroy(bot);
        OnScoreChanged?.Invoke(score);
    }
    
    public void IncreaseBotLimits()
    {
        numberBot1OnScene = Mathf.Min(numberBot1OnScene + botCountIncrement, maxBot1Count);
        numberBot2OnScene = Mathf.Min(numberBot2OnScene + botCountIncrement, maxBot2Count);
        numberBot3OnScene = Mathf.Min(numberBot3OnScene + botCountIncrement, maxBot3Count);
    }
    
    public void IncreaseSpawnRates()
    {
        spawnRateBot1 = Mathf.Min(spawnRateBot1 + spawnRateIncrement, maxSpawnRateBot1);
        spawnRateBot2 = Mathf.Min(spawnRateBot2 + spawnRateIncrement, maxSpawnRateBot2);
        spawnRateBot3 = Mathf.Min(spawnRateBot3 + spawnRateIncrement, maxSpawnRateBot3);
    }
    
    public void IncreaseDifficulty()
    {
        IncreaseBotLimits();
        IncreaseSpawnRates();
    }

    public void ChangeBotMaxCount(int bot1, int bot2, int bot3)
    {
        numberBot1OnScene = bot1;
        numberBot2OnScene = bot2;
        numberBot3OnScene = bot3;
    }

    // 🔥 Обновлено: принимает float
    public void ChangeBotSpawnRate(float bot1, float bot2, float bot3)
    {
        spawnRateBot1 = bot1;
        spawnRateBot2 = bot2;
        spawnRateBot3 = bot3;
    }
    
    public void StopSpawning() => _isSpawningEnabled = false;
    public void ResumeSpawning() => _isSpawningEnabled = true;

    [ContextMenu("Spawn Bot1")]
    public void Spawn() => SpawnBot(bot1, GetSpawnPosition(5f, 5f));
}