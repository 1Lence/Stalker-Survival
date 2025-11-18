using UnityEngine;

public class BulletLogic : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 3f;
    public float damage = 1f;

    void Start()
    {
        // НЕ задаём случайное направление!
        // Просто уничтожаем через время
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Летим вперёд — в направлении, в котором объект повёрнут
        // Если спрайт пули "смотрит вверх" (localScale.y направлен вперёд) — используем transform.up
        // Если "смотрит вправо" — используем transform.right
        transform.position += transform.up * (speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Obstacle"))
        {
            BotBase bot = other.GetComponent<BotBase>();
            if (bot is not null)
            {
                bot.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }

    public void SetDamageAndLife(float dmg, float life)
    {
        damage = dmg;
        lifeTime = life;
    }
}