using UnityEngine;

public class MutantShot : MonoBehaviour
{
    [Header("Основные параметры")]
    [SerializeField] private float lifeTime = 8f; // Время жизни (сек), чтобы не висел вечно
    
    private float damage;
    private float speed;

    private void Start()
    {
        // Автоуничтожение через lifeTime
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        // Летим вперёд в направлении, в котором повёрнут объект
        // Предполагается, что спрайт пули смотрит ВВЕРХ (по оси Y)
        transform.position += transform.up * (speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Наносим урон только игроку
        if (other.CompareTag("Player"))
        {
            PlayerControl player = other.GetComponent<PlayerControl>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        // Уничтожаемся при столкновении с препятствиями (стены, декор и т.д.)
        else if (other.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }

    public void SetStat(float speedInp, float size, float damageInp)
    {
        speed = speedInp;
        damage = damageInp;
        transform.localScale = Vector3.one * size;
    }
}