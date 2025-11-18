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
