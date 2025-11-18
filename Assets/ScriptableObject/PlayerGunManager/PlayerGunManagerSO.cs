// PlayerGunsDataSO.cs
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerGunsData", menuName = "ScriptableObjects/PlayerGunsData", order = 1)]
public class PlayerGunsDataSO : ScriptableObject
{
    [Header("Список оружий и требуемых очков для разблокировки")]
    [SerializeField] private List<GameObject> gunPrefabs = new List<GameObject>();
    [SerializeField] private List<int> scoreRequirements = new List<int>(); // очки для разблокировки КАЖДОГО оружия

    public int GetGunsCount() => gunPrefabs.Count;

    public GameObject GetGunPrefab(int index)
    {
        if (index >= 0 && index < gunPrefabs.Count)
            return gunPrefabs[index];
        return null;
    }

    public int GetScoreRequirement(int index)
    {
        if (index >= 0 && index < scoreRequirements.Count)
            return scoreRequirements[index];
        return int.MaxValue; // оружие недоступно
    }

    // Валидация (опционально, для отладки)
    public bool IsDataValid()
    {
        if (gunPrefabs.Count != scoreRequirements.Count)
        {
            Debug.LogError("PlayerGunsDataSO: количество оружий и требований не совпадает!");
            return false;
        }
        return true;
    }
}