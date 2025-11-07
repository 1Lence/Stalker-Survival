using UnityEngine;

public class PlayerData : MonoBehaviour
{
    private float _health;
    private float _maxHealth = 100L;
    public float _score { get; set; }
    
    void Start()
    {
        _health = _maxHealth;
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            updateScore(100);
            Debug.Log(_score);
        }
    }

    private void updateScore(float newScore)
    {
        _score += newScore;
    }

    public void newLevel()
    {
        _score = 0;
    }
    
    public virtual void TakeDamage(float damageInp)
    {
        if (damageInp >= 0)
        {
            _health -= damageInp;
            _health = Mathf.Clamp(_health, 0f, _maxHealth);
        }
        else
            Debug.LogError($"{nameof(this.gameObject)} - Damage is negative ({damageInp}), can't take!");

        if (_health == 0)
        {
            Destroy(this.gameObject);
        }
    }
}