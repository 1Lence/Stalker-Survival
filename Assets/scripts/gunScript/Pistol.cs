using UnityEngine;
using System.Linq;

public class Pistol : MonoBehaviour, IGun
{
    [SerializeField] private GunDataSO gunDataSo;
    private GameObject bulletPrefab => gunDataSo.bulletPrefab;
    private LayerMask botLayer => gunDataSo.botLayer; // Назначь слой "Bot" в инспекторе
    private float damage => gunDataSo.damagePerBullet;
    private float maxDistance => gunDataSo.maxShootDistance; // Макс. дистанция обнаружения
    private int shootsPerMinutes => gunDataSo.shootPerMinutes;
    private float spread => gunDataSo.spread;
    private float rotationRadius => gunDataSo.rotationRadius;
    public float rotationSpeed => gunDataSo.rotationSpeed;
    
    private float shootInterval = 0f;
    private float lastShotTime;

    private float currentAngle = 0f;

    private Transform nearestBot = null;
    private Transform player; // Родитель = игрок

    void Start()
    {
        player = transform.parent; // Предполагается, что Pistol — дочерний объект игрока
        if (player == null)
        {
            Debug.LogError("Pistol должен быть дочерним объектом Player!");
        }
    }

    void Update()
    {
        FindNearestBot();

        if (nearestBot != null && Vector3.Distance(transform.position, nearestBot.position) <= maxDistance)
        {
            // Режим: есть цель — пистолет стоит НА КРУГЕ напротив цели и смотрит на неё
            PositionAndRotateTowardsTarget(nearestBot.position);
            TryShoot();
        }
        else
        {
            // Нет цели — крутимся свободно вокруг игрока
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
            spawnBoolet();
            lastShotTime = Time.time;
        }
    }

    public void spawnBoolet()
    {
        // Инвертируем поворот на 180°, чтобы пуля летела ВПЕРЁД от ствола
        Quaternion bulletRotation = transform.rotation * Quaternion.Euler(0, 0, 180f);
        Instantiate(bulletPrefab, transform.position, bulletRotation);
        AudioSystem.Instance?.PlayPistolShot();
    }

    // Помещает пистолет на круг вокруг игрока напротив цели и поворачивает на неё
    void PositionAndRotateTowardsTarget(Vector3 targetPosition)
    {
        if (player is null) return;

        // 1. Вычисляем направление от игрока к цели
        Vector3 toTarget = (targetPosition - player.position).normalized;

        // 2. Ставим пистолет на круг в этом направлении (можно добавить смещение, если нужно)
        transform.position = player.position + toTarget * rotationRadius;

        // 3. Поворачиваем пистолет на цель
        float angle = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + 90f); // +90 — подстрой под спрайт
    }

    // Свободное вращение вокруг игрока
    void RotateFreely()
    {
        currentAngle += rotationSpeed * Time.deltaTime;
        float x = Mathf.Cos(currentAngle * Mathf.Deg2Rad) * rotationRadius;
        float y = Mathf.Sin(currentAngle * Mathf.Deg2Rad) * rotationRadius;
        transform.localPosition = new Vector3(x, y, 0);

        // Поворот "вперёд" по касательной или радиусу — как было у тебя
        Vector3 direction = transform.localPosition.normalized;
        float lookAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, lookAngle + 180f);
    }
}