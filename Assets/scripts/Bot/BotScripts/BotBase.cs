using UnityEngine;
using UnityEngine.TestTools;
using static BotsDataSctructures;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Transform))]
public abstract class BotBase : MonoBehaviour
{
    //ScriptableObject
    [SerializeField] protected BotDataSO botDataSO;
    [SerializeField] protected float distanceFromPlayer = 20f; //дистанция от игрока, при которой бот должен телепортироваться
    [SerializeField] private float teleportFromPlayerDistance = 10f; //дистанция от игрока, на которой бот должен заспавнится
    [SerializeField] private float teleportRand = 2f; //рандомизация величины дистанции от игрока при телепорте
    //Тактика бота
    //[SerializeField] protected BotTactic tactic;

    public event System.Action<int, GameObject> OnDeathBot;
    
    protected Rigidbody2D Rb2d;
    protected Transform Transf;
    
    //Transform игрока, чтобы бот знал координаты
    protected Transform PlayerTransform;
    protected Vector2 PlayerPosition => PlayerTransform.position; //? PlayerTransform.position : Vector2.zero;
    protected GameObject PlayerObj;
    protected PlayerControl PlayerControl;

    protected float MaxHealth => this.botDataSO.maxHealth;
    protected float Damage => this.botDataSO.damage;
    protected float MoveSpeed => this.botDataSO.moveSpeed;
    protected float AttackSpeed => this.botDataSO.attackspeed;
    protected float AttackDelay => this.botDataSO.attackDelay;
    protected float AttackDistance => this.botDataSO.attackDistance;
    protected float AttackDistanceTolerance => this.botDataSO.attackDistanceTolerance;
    protected float ShotSpeed => this.botDataSO.shotSpeed;
    protected float ShotSize => this.botDataSO.shotSize;
    public int BotId => this.botDataSO.botID;
    protected int BotScore => this.botDataSO.scoreKill;
    
    protected float CurrentHealth;

    protected virtual void Start()
    {
        if (this.botDataSO is null)
            Debug.LogError($"{nameof(this.gameObject)} Bot DataSO is null");
        
        Transf = GetComponent<Transform>();
        Rb2d = GetComponent<Rigidbody2D>();
        CurrentHealth = this.botDataSO.maxHealth;
    }

    public virtual void TakeDamage(float damageInp)
    {
        if (damageInp >= 0)
        {
            CurrentHealth -= damageInp;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth);
        }
        else
            Debug.LogError($"{nameof(this.gameObject)} - Damage is negative ({damageInp}), can't take!");

        if (CurrentHealth == 0) //смерть
        {
            OnDeathBot?.Invoke(BotScore, gameObject);
        }
    }

    protected void BotTeleportToPlayer()
    {
        if (PlayerTransform is null)
        {
            Debug.LogWarning("BotTeleportToPlayer: PlayerTransform is null!");
            return;
        }

        // Вычисляем новую позицию: случайная точка на окружности вокруг игрока
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float distance = teleportFromPlayerDistance + Random.Range(-teleportRand, teleportRand);
    
        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;
        Vector3 newPosition = PlayerTransform.position + new Vector3(offset.x, offset.y, 0);

        // Применяем новую позицию
        transform.position = newPosition;

        // Если у бота есть Rigidbody2D — обновляем его позицию тоже
        if (Rb2d is not null)
        {
            Rb2d.MovePosition(newPosition);
        }

        //Debug.Log($"Bot {gameObject.name} teleported to {newPosition}");
    }

    public virtual void SetPlayerTransform(Transform player)
    {
        PlayerTransform = player;
        // if (player is not null)
        //     PlayerTransform = player;
        // else
        //     Debug.LogError($"{nameof(this.gameObject)}  - PlayerTransform is null! Error in SetPlayerTransform!");
    }

    public virtual void SetPlayerControl(PlayerControl player)
    {
        PlayerControl = player;
        // if (player is not null)
        //     PlayerControl = player;
        // else
        //     Debug.LogError($"{nameof(this.gameObject)} - PlayerObj is null! Error in SetPlayerObject!");
    }
    
    public float GetCurrentHealth() => CurrentHealth;
    public float GetMaxHealth() => MaxHealth;
    public Transform GetPlayerTransform() => PlayerTransform;
    
    [ContextMenu("Kill Bot")]
    public void KillBot()
    {
        TakeDamage(MaxHealth);
    }
}
