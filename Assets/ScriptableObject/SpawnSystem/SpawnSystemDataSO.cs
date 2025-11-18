using UnityEngine;

[CreateAssetMenu(fileName = "SpawnSystemDataSO", menuName = "Scriptable Objects/SpawnSystemDataSO")]
public class SpawnSystemDataSO : ScriptableObject
{
    [Header("Префабы ботов")]
    public GameObject bot1;
    public GameObject bot2;
    public GameObject bot3;
    
    [Header("Настройки спавна")]
    public int numberBot1OnScene;
    public int numberBot2OnScene;
    public int numberBot3OnScene;
    
    public float spawnRateBot1 = 1f; // ботов в секунду (может быть 0.1, 0.5, 2.3 и т.д.)
    public float spawnRateBot2 = 1f;
    public float spawnRateBot3 = 1f;
    
    public float spawnDistanceFromPlayer = 10f;
    public float spawnDistanceFromPlayerRand = 2f;
    
    [Header("Настройки роста сложности")]
    public float difficultyIncreaseInterval = 10f;
    public int botCountIncrement = 1;
    public float spawnRateIncrement = 0.2f; 

    [Header("Максимумы (ограничения сложности)")]
    public int maxBot1Count = 5;
    public int maxBot2Count = 5;
    public int maxBot3Count = 5;
    
    public float maxSpawnRateBot1 = 5f;
    public float maxSpawnRateBot2 = 5f; 
    public float maxSpawnRateBot3 = 5f;
}

