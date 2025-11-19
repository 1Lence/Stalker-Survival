using UnityEngine;

public class MutantLogic : BotBase
{
    [SerializeField] private Animator animator;
    private Vector2 _direction = Vector2.zero;
    private float _nextAttackTime = 0f;
    private bool _isMoving = false;
    private bool _isAttacking = false; // 🔥 Новый флаг

    protected override void Start()
    {
        base.Start();
    }

    void Update()
    { 
        if (PlayerTransform is not null)
        {
            BotLogic(PlayerPosition);
            
            float distanceToPlayer = Vector2.Distance(transform.position, PlayerTransform.position);
            if (distanceToPlayer > distanceFromPlayer) // distanceFromPlayer — из BotBase
            {
                BotTeleportToPlayer();
            }
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

        float maxHitDistance = AttackDistance + AttackDistanceTolerance;

        if (distance > maxHitDistance)
        {
            _isMoving = true;
            _isAttacking = false;
            CancelInvoke(nameof(PerformAttack)); // Отменяем, если ушли из зоны
            animator.SetBool("IsMoving", true);
        }
        else
        {
            _isMoving = false;
            animator.SetBool("IsMoving", false);

            // Атакуем только если не в процессе атаки и прошло время
            if (!_isAttacking && Time.time >= _nextAttackTime)
            {
                _isAttacking = true;
                CancelInvoke(nameof(PerformAttack)); // На всякий случай
                Invoke(nameof(PerformAttack), AttackDelay);
            }
        }
    }

    private void PerformAttack()
    {
        if (PlayerTransform != null)
        {
            float distance = Vector2.Distance(transform.position, PlayerTransform.position);
            float maxHitDistance = AttackDistance + AttackDistanceTolerance;

            if (distance <= maxHitDistance)
            {
                PlayerControl?.TakeDamage(Damage);

                switch (BotId)
                {
                    case 1:
                        AudioSystem.Instance?.PlayMutantAtackBig();
                        break;
                    case 2:
                        AudioSystem.Instance?.PlayMutantAtackSmall();
                        break;
                    case 3:
                        AudioSystem.Instance?.PlayMutantShot();
                        break;
                }
            }
        }

        //Сбрасываем флаг и устанавливаем таймер
        _isAttacking = false;
        _nextAttackTime = Time.time + (1f / AttackSpeed);
    }

    // На всякий случай: отменяем при уничтожении
    protected void OnDestroy()
    {
        CancelInvoke(nameof(PerformAttack));
    }
}