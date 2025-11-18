using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private PlayerDataSO playerDataSO;
    private float maxHealth => playerDataSO.maxHealth;
    private float moveSpeed => playerDataSO.speed;
    
    [SerializeField] private Rigidbody2D rb;

    [Header("Aiming")]
    [SerializeField] private Transform aimTarget; // Объект, который будет поворачиваться (например, дочерний спрайт или пушка)
    [SerializeField] private LayerMask botLayer;  // Слой "Bot"
    [SerializeField] private float maxAimDistance = 20f; // Макс. дистанция, на которой игрок смотрит на бота

    public Vector3 playerMoveDirection;
    public System.Action OnDeath;
    public System.Action<float, float, float> OnHealthChange;

    private float _currentHealth;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        _currentHealth = maxHealth;
        if (aimTarget is null)
        {
            aimTarget = transform; // если не назначен — поворачиваем всё тело
        }
    }

    void Update()
    {
        // Управление движением
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");
        playerMoveDirection = new Vector3(inputX, inputY).normalized;

        // Поворот: сначала пробуем на бота, если нет — по движению
        LookAtTarget();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = playerMoveDirection * moveSpeed;
    }

    void LookAtTarget()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, maxAimDistance, botLayer);

        if (colliders.Length > 0)
        {
            // Есть боты — ищем ближайшего
            Transform nearest = null;
            float nearestDist = Mathf.Infinity;

            foreach (var col in colliders)
            {
                float dist = Vector3.Distance(transform.position, col.transform.position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = col.transform;
                }
            }

            if (nearest is not null)
            {
                Vector3 direction = (nearest.position - transform.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                aimTarget.rotation = Quaternion.Euler(0, 0, angle + 90f);
                return; // Успешно навелись на бота — выходим
            }
        }

        // Если нет ботов ИЛИ что-то пошло не так — смотрим по направлению движения
        LookInMoveDirection();
    }

    void LookInMoveDirection()
    {
        if (playerMoveDirection != Vector3.zero)
        {
            float angle = Mathf.Atan2(playerMoveDirection.y, playerMoveDirection.x) * Mathf.Rad2Deg;
            aimTarget.rotation = Quaternion.Euler(0, 0, angle + 90f);
        }
        // Если playerMoveDirection == zero — оставляем текущее направление
    }

    public void TakeDamage(float damage)
    {
        if (_currentHealth > 0)
        {
            _currentHealth -= damage;
            _currentHealth = Mathf.Clamp(_currentHealth, 0, maxHealth);
            OnHealthChange?.Invoke(_currentHealth, maxHealth, _currentHealth / maxHealth);
        }

        if (_currentHealth <= 0)
        {
            OnDeath?.Invoke();
        }
    }

    public void TakeHeal(float heal)
    {
        if (_currentHealth <= 0)
            return;
        else if (heal > 0)
        {
            _currentHealth += heal;
            _currentHealth = Mathf.Clamp(_currentHealth, 0, maxHealth);
            OnHealthChange?.Invoke(_currentHealth, maxHealth, _currentHealth / maxHealth);
        }
    }
}