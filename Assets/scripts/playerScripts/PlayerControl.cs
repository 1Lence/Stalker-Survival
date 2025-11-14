using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Rigidbody2D rb;
    public Vector3 playerMoveDirection;
    
    public System.Action OnDeath;
    
    public System.Action<float, float, float> OnHealthChange;
    
    private float _currentHealth;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        _currentHealth = maxHealth;
    }

    void Update()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");
        playerMoveDirection = new Vector3(inputX, inputY).normalized;
    }
    
    void FixedUpdate(){
        rb.linearVelocity = new Vector3(playerMoveDirection.x * moveSpeed, playerMoveDirection.y * moveSpeed);
    }

    public void TakeDamage(float damage)
    {
        if (_currentHealth > 0)
        {
            _currentHealth -= damage;
            _currentHealth =  Mathf.Clamp(_currentHealth, 0, maxHealth);
            OnHealthChange?.Invoke(_currentHealth, maxHealth, _currentHealth/maxHealth);
        }
        
        if (_currentHealth <= 0)
        {
            OnDeath?.Invoke();
        }
        //Debug.Log($"{nameof(this.TakeDamage)} - Damage Received: {damage} !");
    }
}