using UnityEngine;

public class BulletLogic : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 3f;
    private long damage; // TODO: Добавить изменяемый урон

    void Start()
    {
        float randomAngle = Random.Range(0f, 360f);
        
        Vector2 randomDirection = Quaternion.Euler(0, 0, randomAngle) * Vector2.up;
        
        transform.up = randomDirection;
        
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += transform.right * (speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //TODO: Обработка столкновений
        if (other.CompareTag("Enemy") || other.CompareTag("Obstacle"))
        {
            BotBase bot = other.GetComponent<BotBase>();
            if (bot is not null)
            {
                bot.TakeDamage(15f);
            }
            
            Destroy(gameObject);
        }
    }
}