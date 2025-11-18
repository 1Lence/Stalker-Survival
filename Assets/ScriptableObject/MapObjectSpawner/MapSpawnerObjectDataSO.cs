using UnityEngine;

[CreateAssetMenu(fileName = "MapSpawnerObjectDataSO", menuName = "Scriptable Objects/MapSpawnerObjectDataSO")]
public class MapSpawnerObjectDataSO : ScriptableObject
{
    [Header("Объекты для спавна")]
    public GameObject healPrefab; // Префаб аптечки

    [Header("Общие настройки")]
    public int initialCount = 3;        // Начальное количество
    public int maxObjectsOnMap = 5;      // Максимум на карте
    public float spawnInterval = 10f;    // Интервал между спавнами (сек)
    public int spawnBatchSize = 1;       // Сколько объектов спавнить за раз

    [Header("Настройки позиции")]
    public float minDistanceBetween = 3f; // Мин. расстояние между объектами
}