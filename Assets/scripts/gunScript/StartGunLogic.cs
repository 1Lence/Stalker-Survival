// GunManager.cs
using UnityEngine;

public class GunManager : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private PlayerGunsDataSO gunsData;
    [SerializeField] private GameObject player;

    private PlayerData _playerData;
    private int _currentGunIndex = 0; // текущее оружие (начинаем с 0)

    private void Start()
    {
        if (gunsData == null)
        {
            Debug.LogError("GunManager: gunsData not assigned!");
            return;
        }

        if (!gunsData.IsDataValid())
        {
            Debug.LogError("GunManager: gunsData is invalid!");
            return;
        }

        _playerData = player?.GetComponent<PlayerData>();
        if (_playerData == null)
        {
            Debug.LogError("GunManager: PlayerData not found on player!");
            return;
        }

        // Спавним первое оружие
        SpawnGun(_currentGunIndex);
    }

    private void Update()
    {
        // Проверяем, можно ли разблокировать следующее оружие
        if (_currentGunIndex + 1 < gunsData.GetGunsCount())
        {
            int requiredScore = gunsData.GetScoreRequirement(_currentGunIndex + 1);
            if (_playerData.GetScore() >= requiredScore)
            {
                UnlockNextGun();
            }
        }
    }

    private void UnlockNextGun()
    {
        _currentGunIndex++;
        SpawnGun(_currentGunIndex);

        Debug.Log($"GunManager: Unlocked gun {_currentGunIndex + 1}");
    }

    private void SpawnGun(int index)
    {
        GameObject prefab = gunsData.GetGunPrefab(index);
        if (prefab == null)
        {
            Debug.LogError($"GunManager: Gun prefab at index {index} is null!");
            return;
        }

        // Удаляем предыдущее оружие (если нужно)
        // Или оставляем все? В твоём примере — добавляются все.
        // Здесь реализуем как в оригинале: все оружия остаются.

        GameObject gunObj = Instantiate(prefab, transform.position, Quaternion.identity);
        gunObj.transform.SetParent(transform);
    }

    // Публичный метод для отладки или внешнего управления
    public int GetCurrentGunIndex() => _currentGunIndex;
}