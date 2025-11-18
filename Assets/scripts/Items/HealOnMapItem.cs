// HealItem.cs
using UnityEngine;

public class HealItem : MonoBehaviour, IHealable
{
    [SerializeField] private float _healAmount = 15f;
    
    public System.Action<GameObject> OnDestroyed { get; set; }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Лечим игрока
            other.GetComponent<PlayerControl>()?.TakeHeal(_healAmount);
            
            // Уведомляем спавнер
            OnDestroyed?.Invoke(gameObject);
        }
    }
}
