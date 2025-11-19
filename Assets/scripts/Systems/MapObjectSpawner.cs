using System.Collections.Generic;
using UnityEngine;

public class MapObjectSpawner : MonoBehaviour
{
    [SerializeField] MapSpawnerObjectDataSO spawnerDataSO;
    
    private GameObject healPrefab => spawnerDataSO.healPrefab; // Префаб аптечки
    
    private int initialCount => spawnerDataSO.initialCount;        // Начальное количество
    private int maxObjectsOnMap => spawnerDataSO.maxObjectsOnMap;      // Максимум на карте
    private float spawnInterval => spawnerDataSO.spawnInterval;    // Интервал между спавнами (сек)
    private int spawnBatchSize => spawnerDataSO.spawnBatchSize;       // Сколько объектов спавнить за раз
    
    private float minDistanceBetween => spawnerDataSO.minDistanceBetween; // Мин. расстояние между объектами
     private int maxSpawnAttempts = 20;     // Попытки найти валидную позицию

    private List<GameObject> activeObjects = new List<GameObject>();
    private float lastSpawnTime = 0f;
    private BoxCollider2D mapBounds;

    void Start()
    {
        mapBounds = GetComponent<BoxCollider2D>();
        if (mapBounds == null)
        {
            Debug.LogError("MapObjectSpawner: BoxCollider2D не найден на карте!");
            return;
        }

        // Спавним начальное количество
        for (int i = 0; i < initialCount && activeObjects.Count < maxObjectsOnMap; i++)
        {
            SpawnObject();
        }

        lastSpawnTime = Time.time;
    }

    void Update()
    {
        // Динамический спавн, если есть место
        if (Time.time - lastSpawnTime >= spawnInterval && activeObjects.Count < maxObjectsOnMap)
        {
            for (int i = 0; i < spawnBatchSize && activeObjects.Count < maxObjectsOnMap; i++)
            {
                SpawnObject();
            }
            lastSpawnTime = Time.time;
        }
    }

    private void SpawnObject()
    {
        Vector2 spawnPos = GetValidSpawnPosition();
        if (spawnPos != Vector2.zero)
        {
            GameObject obj = Instantiate(healPrefab, spawnPos, Quaternion.identity);
            activeObjects.Add(obj);

            // Подписываемся на событие уничтожения (если есть)
            var healable = obj.GetComponent<IHealable>();
            if (healable != null)
            {
                healable.OnDestroyed += OnObjectDestroyed;
            }
            // Или, если у тебя событие в самом скрипте аптечки:
            // var healScript = obj.GetComponent<HealItem>();
            // if (healScript != null) healScript.OnPickup += OnObjectDestroyed;
        }
    }

    private Vector2 GetValidSpawnPosition()
    {
        for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            // Генерируем случайную позицию внутри BoxCollider2D
            Vector2 randomPoint = new Vector2(
                Random.Range(mapBounds.bounds.min.x, mapBounds.bounds.max.x),
                Random.Range(mapBounds.bounds.min.y, mapBounds.bounds.max.y)
            );

            // Проверка: не слишком ли близко к другим объектам?
            bool tooClose = false;
            foreach (var obj in activeObjects)
            {
                if (obj is null) continue; // уничтожен, но не удалён из списка
                if (Vector2.Distance(obj.transform.position, randomPoint) < minDistanceBetween)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
            {
                return randomPoint;
            }
        }

        // Не удалось найти место
        Debug.LogWarning("MapObjectSpawner: не удалось найти валидную позицию для спавна");
        return Vector2.zero;
    }

    private void OnObjectDestroyed(GameObject obj)
    {
        activeObjects.Remove(obj);
        Destroy(obj);
    }

    // Публичный метод для ручного спавна (опционально)
    public void ForceSpawn()
    {
        if (activeObjects.Count < maxObjectsOnMap)
        {
            SpawnObject();
        }
    }

    // // Для отладки
    // private void OnDrawGizmosSelected()
    // {
    //     if (mapBounds is not null)
    //     {
    //         Gizmos.color = Color.green;
    //         Gizmos.DrawWireCube(mapBounds.bounds.center, mapBounds.bounds.size);
    //     }
    // }
}

public interface IHealable
{
    System.Action<GameObject> OnDestroyed { get; set; }
}