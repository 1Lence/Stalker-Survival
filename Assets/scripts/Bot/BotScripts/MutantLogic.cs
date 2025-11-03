using UnityEngine;

public class MutantLogic : BotBase
{
    private Vector2 _direction = Vector2.zero;
    
    protected override void Start()
    {
        base.Start();
    }
    
    void Update()
    { 
        if (PlayerTransform is not null)
            BotLogic(PlayerPosition);
    }

    private void BotMove()
    {
        Rb2d.MovePosition(Rb2d.position + MoveSpeed * Time.deltaTime * _direction);
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
        
        if (distance > AttackDistance)
        {
            BotMove();
        }
        else
        {
            Attack();
        }
    }

    private void Attack()
    {
        
    }
}
