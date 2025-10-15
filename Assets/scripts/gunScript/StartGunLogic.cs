using UnityEngine;

public class StartGunLogic : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    private Transform spawnPoint;
    public float shootInterval = 1f; // Интервал между выстрелами

    private float lastShotTime;

    void Update()
    {
        spawnPoint = transform;
        if (Time.time - lastShotTime >= shootInterval)
        {
            SpawnBullet();
            lastShotTime = Time.time;
        }
    }

    void SpawnBullet()
    {
        Instantiate(bulletPrefab, spawnPoint.position, Quaternion.identity);
    }
}