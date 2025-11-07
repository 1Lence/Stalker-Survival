using UnityEngine;

public class ARGun : MonoBehaviour, IGun
{
    [SerializeField] private GameObject bulletPrefab;

    private Transform spawnPoint;

    private float shootInterval = 0.5f; // Интервал между выстрелами

    private float lastShotTime;

    private float rotationRadius = 1f;
    public float rotationSpeed = 90f;

    private float currentAngle = 0f;

    void Update()
    {
        spawnPoint = transform;
        rotate();
        if (Time.time - lastShotTime >= shootInterval)
        {
            spawnBoolet();
            lastShotTime = Time.time;
        }
    }

    public void spawnBoolet()
    {
        Instantiate(bulletPrefab, spawnPoint.position, Quaternion.identity);
    }

    public void rotate()
    {
        currentAngle += rotationSpeed * Time.deltaTime;

        float x = Mathf.Cos(currentAngle * Mathf.Deg2Rad) * rotationRadius;
        float y = Mathf.Sin(currentAngle * Mathf.Deg2Rad) * rotationRadius;

        transform.localPosition = new Vector3(x, y, 0);

        Vector3 direction = transform.localPosition.normalized;
        float lookAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, lookAngle + 180f);
    }
}