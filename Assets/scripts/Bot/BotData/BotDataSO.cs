using UnityEngine;

[CreateAssetMenu(fileName = "BotDataSO", menuName = "Scriptable Objects/BotDataSO")]
public class BotDataSO : ScriptableObject
{
    [Header("Basic Stats")]
    public string botName;
    public float maxHealth = 100f;
    public float damage = 20f;
    public float moveSpeed = 3f;
    public float attackspeed = 1f;
    public float attackDistance = 1f;
    public float scoreKill = 10f;
}
