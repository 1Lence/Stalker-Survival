using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private RectTransform fillBar;
    
    [SerializeField] private PlayerControl playerControl;

    void Start()
    {
        playerControl.OnHealthChange += OnBarChangeAmount;
    }

    public void OnBarChangeAmount(float currentHealth, float maxHealth, float healthPercentage)
    {
        fillBar.localScale = new Vector3(healthPercentage, 1, 1);
    }
}
