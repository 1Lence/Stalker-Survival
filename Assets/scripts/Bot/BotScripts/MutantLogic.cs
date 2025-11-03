using UnityEngine;

public class MutantLogic : BotBase
{
    [SerializeField] private Transform playerTrans; //временная заглушка, т.к. спавнера бота нет.
    
    private Vector2 direction = Vector2.zero;
    
    protected override void Start()
    {
        base.Start();
        SetPlayerTransform(playerTrans);
    }
    
    void Update()
    { 
        if (playerTransform is not null)
            BotLogic(playerPosition);
    }

    private void BotMove()
    {
        rb2d.MovePosition(rb2d.position + moveSpeed * Time.deltaTime * direction);
    }

    private void BotDirection(Vector2 target)
    {
        direction = (target - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }

    private void BotLogic(Vector2 target)
    {
        float distance = Vector2.Distance(transform.position, target);
        BotDirection(target);
        
        if (distance > attackDistance)
        {
            BotMove();
        }
        else
        {
            BotAttack(target);
        }
    }

    private void BotAttack(Vector2 target)
    {
        
    }
}
