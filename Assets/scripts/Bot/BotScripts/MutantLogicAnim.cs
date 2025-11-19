using UnityEngine;

public class MutantLogicAnim : BotBase
{
    [SerializeField] private Animator animator;
    
    private Vector2 _direction = Vector2.zero;
    private float _nextAttackTime = 0f;
    private bool _isMoving = false;
    private bool _isAttacking = false;

    protected override void Start()
    {
        base.Start();
        
        if (animator is null)
            animator = GetComponent<Animator>();
    }

    void Update()
    { 
        if (PlayerTransform is not null)
        {
            BotLogic(PlayerPosition);
            
            float distanceToPlayer = Vector2.Distance(transform.position, PlayerTransform.position);
            if (distanceToPlayer > distanceFromPlayer)
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
            //Убираем CancelInvoke, потому что PerformAttack больше не вызывается

            animator.SetBool("IsMoving", true);
        }
        else
        {
            _isMoving = false;

            if (!_isAttacking && Time.time >= _nextAttackTime)
            {
                _isAttacking = true;
                // Запускаем анимацию атаки
                animator.SetBool("IsMoving", false);
                animator.SetTrigger("AttackTrigger");
            }
            else
            {
                animator.SetBool("IsMoving", false);
            }
        }
    }

    //Вызывается из Animation Event в нужный момент атаки
    public void OnAttackHit()
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
    }
    
    public void OnAttackEnd()
    {
        _isAttacking = false;
        _nextAttackTime = Time.time + (1f / AttackSpeed);
    }
}
