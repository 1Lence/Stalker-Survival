// HealItem.cs
using UnityEngine;

public class HealItem : MonoBehaviour, IHealable
{
    [SerializeField] private HealDataSO healDataSO;

    private int healAmount => healDataSO.healAmount;
    
    public System.Action<GameObject> OnDestroyed { get; set; }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Лечим игрока
            other.GetComponent<PlayerControl>()?.TakeHeal(healAmount);
            
            // Уведомляем спавнер
            OnDestroyed?.Invoke(gameObject);
        }
    }
}
