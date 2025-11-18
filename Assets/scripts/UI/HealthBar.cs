using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private RectTransform fillBar;
    
    [SerializeField] private PlayerControl playerControl;

    void Start()
    {
        fillBar.localScale = new Vector3(1, 1, 1);
        fillBar.localPosition = new Vector3(0, 0, -1);
    }

    public void OnBarChangeAmount(float currentHealth, float maxHealth, float healthPercentage)
    {
        fillBar.localScale = new Vector3(healthPercentage, 1, 1);
        fillBar.localPosition = new Vector3((healthPercentage - 1)/2, 0, -1);
    }
}
