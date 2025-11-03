using System;
using UnityEngine;
using static BotsDataSctructures;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Transform))]
public abstract class BotBase : MonoBehaviour
{
    [SerializeField] protected BotDataSO botSOData;
    [SerializeField] protected BotTactic tactic;
    
    protected Rigidbody2D rb2d;
    protected Transform transf;
    
    protected Transform playerTransform;
    protected Vector2 playerPosition => playerTransform is not null ? playerTransform.position : Vector2.zero;

    protected float maxHealth => botSOData.maxHealth;
    protected float damage => botSOData.damage;
    protected float moveSpeed => botSOData.moveSpeed;
    protected float attackSpeed => botSOData.attackspeed;
    protected float attackDistance => botSOData.attackDistance;
    
    protected float currentHealth;

    protected virtual void Start()
    {
        transf = GetComponent<Transform>();
        rb2d = GetComponent<Rigidbody2D>();
        currentHealth = botSOData.maxHealth;
    }

    public virtual void TakeDamage(float damageInp)
    {
        if (damageInp >= 0)
        {
            currentHealth -= damageInp;
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        }
        else
            Debug.LogError($"{nameof(this.gameObject)} - Damage is negative ({damageInp}), can't take!");

        if (currentHealth == 0)
        {
            Destroy(this.gameObject);
        }
    }

    public virtual void SetPlayerTransform(Transform player)
    {
        if (player is not null)
            playerTransform = player;
        else
            Debug.LogError($"{nameof(this.gameObject)}  - PlayerTransform is null! Error in SetPlayerTransform!");
    }
    
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
}
