using UnityEngine;
using System.Collections.Generic;

public class GunManager : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private PlayerGunsDataSO gunsData;
    [SerializeField] private GameObject player;

    private PlayerData _playerData;
    private int _currentGunIndex = 0;
    private List<GameObject> _spawnedGuns = new List<GameObject>(); // 🔥 Список всех оружий

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
        UpdateGunPositions(); // 🔥 Распределяем оружия по кругу

        Debug.Log($"GunManager: Unlocked gun {_currentGunIndex + 1}");
    }

    private void SpawnGun(int index)
    {
        GameObject prefab = gunsData.GetGunPrefab(index);
        if (prefab is null)
        {
            Debug.LogError($"GunManager: Gun prefab at index {index} is null!");
            return;
        }

        GameObject gunObj = Instantiate(prefab, transform.position, Quaternion.identity);
        gunObj.transform.SetParent(transform);

        _spawnedGuns.Add(gunObj);
    }

    // 🔥 Распределяем все оружия по кругу
    private void UpdateGunPositions()
    {
        if (_spawnedGuns.Count == 0) return;

        float angleStep = 360f / _spawnedGuns.Count; // Угол между оружиями

        for (int i = 0; i < _spawnedGuns.Count; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle);
            float y = Mathf.Sin(angle);

            Vector3 position = new Vector3(x, y, 0) * 1f; // 1f — радиус (можно вынести в настройки)
            Quaternion rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg + 90f); // +90 если спрайт смотрит вверх

            // Вызываем метод SetPosition, если он есть
            var gunComponent = _spawnedGuns[i].GetComponent<Gun>();
            if (gunComponent is not null)
            {
                gunComponent.SetPosition(position, rotation);
            }
        }
    }

    public int GetCurrentGunIndex() => _currentGunIndex;

    // 🔥 (опционально) Удаляет оружие из списка, если оно уничтожено
    public void OnGunDestroyed(GameObject gun)
    {
        if (_spawnedGuns.Contains(gun))
        {
            _spawnedGuns.Remove(gun);
            UpdateGunPositions(); // перераспределяем
        }
    }
}