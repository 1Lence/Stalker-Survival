using UnityEngine;

[CreateAssetMenu(fileName = "BotDataSO", menuName = "Scriptable Objects/BotDataSO")]
public class BotDataSO : ScriptableObject
{
    [Header("Basic Stats")]
    public string botName;
    public int botID;
    public float maxHealth = 100f;
    public float damage = 20f;
    public float moveSpeed = 3f;
    public float attackspeed = 1f;
    public float attackDelay = 0.5f;
    public float attackDistance = 1f;
    public float attackDistanceTolerance = 0.5f;
    public float shotSpeed = 3f;
    public float shotSize = 1f;
    public int scoreKill = 10;
}
