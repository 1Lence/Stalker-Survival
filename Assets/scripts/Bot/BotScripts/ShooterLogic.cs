using UnityEngine;

public class ShooterLogic : BotBase
{
    [Header("Shooter Settings")]
    [SerializeField] private GameObject bulletPrefab; // Префаб пули

    private Vector2 _direction = Vector2.zero;
    private float _nextAttackTime = 0f;
    private bool _isMoving = false;
    private bool _isAttacking = false;

    protected override void Start()
    {
        base.Start();
        if (bulletPrefab == null)
        {
            Debug.LogError("ShooterLogic: bulletPrefab not assigned!");
        }
    }

    void Update()
    { 
        if (PlayerTransform is not null)
        {
            BotLogic(PlayerPosition);
        }
    }

    void FixedUpdate()
    {
        if (_isMoving)
            BotMove();
    }

    private void BotMove()
    {
        Rb2d.MovePosition(Rb2d.position + MoveSpeed * Time.fixedDeltaTime * _direction);
    }

    private void BotDirection(Vector2 target)
    {
        _direction = (target - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }

    private void BotLogic(Vector2 target)
    {
        float distance = Vector2.Distance(transform.position, target);
        BotDirection(target);

        float maxAttackDistance = AttackDistance + AttackDistanceTolerance;

        if (distance > maxAttackDistance)
        {
            // Игрок вне зоны — движемся
            _isMoving = true;
            _isAttacking = false;
            CancelInvoke(nameof(PerformAttack));
        }
        else
        {
            // Игрок в зоне — останавливаемся и готовим выстрел
            _isMoving = false;

            if (!_isAttacking && Time.time >= _nextAttackTime)
            {
                _isAttacking = true;
                CancelInvoke(nameof(PerformAttack));
                Invoke(nameof(PerformAttack), AttackDelay);
            }
        }
    }

    private void PerformAttack()
    {
        if (PlayerTransform == null || bulletPrefab == null)
        {
            _isAttacking = false;
            return;
        }

        // Проверяем, что игрок всё ещё в зоне поражения
        float distance = Vector2.Distance(transform.position, PlayerTransform.position);
        float maxHitDistance = AttackDistance + AttackDistanceTolerance;

        if (distance <= maxHitDistance)
        {
            // Направление на игрока
            Vector2 shootDirection = (PlayerTransform.position - transform.position).normalized;
            
            // Угол для поворота пули (если спрайт пули смотрит вверх — используем +90)
            float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
            Quaternion bulletRotation = Quaternion.Euler(0, 0, angle + 270f);

            // Создаём пулю
            GameObject bullet = Instantiate(bulletPrefab, transform.position, bulletRotation);

            // Опционально: настройка скорости и размера через BotDataSO
            var bulletScript = bullet.GetComponent<MutantShot>();
            if (bulletScript != null)
            {
                bulletScript.SetStat(ShotSpeed, ShotSize, Damage);
            }

            // Звук выстрела
            switch (BotId)
            {
                case 1:
                    AudioSystem.Instance?.PlayMutantAtackBig(); // или специальный звук
                    break;
                case 2:
                    AudioSystem.Instance?.PlayMutantAtackSmall();
                    break;
                case 3:
                    AudioSystem.Instance?.PlayMutantShot();
                    break;
            }
        }

        _isAttacking = false;
        _nextAttackTime = Time.time + (1f / AttackSpeed);
    }

    protected void OnDestroy()
    {
        CancelInvoke(nameof(PerformAttack));
    }
}
