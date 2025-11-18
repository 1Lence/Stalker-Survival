using UnityEngine;
using System.Linq;

public class Gun : MonoBehaviour
{
    [SerializeField] private GunDataSO gunDataSo;

    private GameObject bulletPrefab => gunDataSo.bulletPrefab;
    private LayerMask botLayer => gunDataSo.botLayer;
    private float damage => gunDataSo.damagePerBullet;
    private float maxDistance => gunDataSo.maxShootDistance;
    private int shootsPerMinutes => gunDataSo.shootPerMinutes;
    private float spread => gunDataSo.spread;
    private int bulletsPerShot => gunDataSo.bulletsPerShot;
    private float bulletLife => gunDataSo.bulletLife;
    private AudioClip bulletSound => gunDataSo.bulletSound;

    private float shootInterval;
    private float lastShotTime;

    private float rotationRadius => gunDataSo.rotationRadius;
    public float rotationSpeed => gunDataSo.rotationSpeed;
    private float currentAngle = 0f;

    private Transform nearestBot = null;
    private Transform player; // Игрок = родитель
    private bool _audioSystemReady = false;

    void Start()
    {
        player = transform.parent;
        if (player is null)
        {
            Debug.LogError("Gun должен быть дочерним объектом Player!");
        }

        if (shootsPerMinutes > 0)
        {
            shootInterval = 60f / shootsPerMinutes;
        }
        else
        {
            shootInterval = float.MaxValue;
        }
        
        // Проверяем наличие AudioSystem в первый кадр
        if (!_audioSystemReady)
        {
            _audioSystemReady = AudioSystem.Instance is not null;
            if (!_audioSystemReady)
            {
                // Попытка найти на сцене (на случай, если не синглтон или загружен позже)
                var audioSystem = FindObjectOfType<AudioSystem>();
                if (audioSystem is not null)
                {
                    _audioSystemReady = true;
                }
                // Если не найден — игнорируем звук, но не падаем
            }
        }
    }

    void Update()
    {
        FindNearestBot();

        if (nearestBot is not null && Vector3.Distance(transform.position, nearestBot.position) <= maxDistance)
        {
            PositionAndRotateTowardsTarget(nearestBot.position);
            TryShoot();
        }
        else
        {
            RotateFreely();
        }
    }

    void FindNearestBot()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, maxDistance, botLayer);
        nearestBot = colliders
            .Select(c => c.transform)
            .OrderBy(t => Vector3.Distance(transform.position, t.position))
            .FirstOrDefault();
    }

    void TryShoot()
    {
        if (Time.time - lastShotTime >= shootInterval)
        {
            SpawnBullet();
            lastShotTime = Time.time;
        }
    }

    public void SpawnBullet()
    {
        Vector2 baseDirection = transform.up;

        if (bulletsPerShot <= 1)
        {
            FireSingleBullet(baseDirection, spread);
        }
        else
        {
            float totalSpread = spread;
            float step = bulletsPerShot > 1 ? totalSpread / (bulletsPerShot - 1) : 0f;
            float startAngle = -totalSpread / 2f;

            for (int i = 0; i < bulletsPerShot; i++)
            {
                float angleOffset = startAngle + i * step;
                FireSingleBullet(baseDirection, angleOffset);
            }
        }

        // 🔊 Безопасное проигрывание звука
        if (_audioSystemReady && bulletSound is not null)
        {
            AudioSystem.Instance.PlayOneShot(bulletSound);
        }
        // else if (bulletSound is not null)
        // {
        //     // Fallback: если AudioSystem не готов, но звук есть — проигрываем напрямую
        //     //AudioSource.PlayClipAtPoint(bulletSound, transform.position, 0.7f);
        // }
    }

    private void FireSingleBullet(Vector2 baseDirection, float angleOffsetDegrees)
    {
        float angleRad = angleOffsetDegrees * Mathf.Deg2Rad;
        Vector2 shootDirection = new Vector2(
            baseDirection.x * Mathf.Cos(angleRad) - baseDirection.y * Mathf.Sin(angleRad),
            baseDirection.x * Mathf.Sin(angleRad) + baseDirection.y * Mathf.Cos(angleRad)
        ).normalized;

        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        Quaternion bulletRotation = Quaternion.Euler(0, 0, angle + 90f);

        GameObject bulletObj = Instantiate(bulletPrefab, transform.position, bulletRotation);
        var bullet = bulletObj.GetComponent<BulletLogic>();
        if (bullet is not null)
        {
            bullet.SetDamageAndLife(damage, bulletLife);
        }
    }

    void PositionAndRotateTowardsTarget(Vector3 targetPosition)
    {
        if (player is null) return;

        Vector3 toTarget = (targetPosition - player.position).normalized;
        transform.position = player.position + toTarget * rotationRadius;

        float angle = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + 90f);
    }

    void RotateFreely()
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