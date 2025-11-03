using UnityEngine;

public class SpawnSystem : MonoBehaviour
{
    public static SpawnSystem Instance { get; private set; }
    
    [SerializeField] private GameObject bot;

    private GameObject _player;
    
    void Awake()
    {
        if (Instance is null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
        
        _player = GameObject.FindWithTag("Player");

        if (_player is null)
            Debug.LogError("Player not found");
    }

    private void SpawnBot(GameObject botPrefab, Vector2 position)
    {
        GameObject botGo = Instantiate(botPrefab, position, Quaternion.identity);
        
        BotBase botObj = botGo.GetComponent<BotBase>();
        if (bot is not null)
        {
            botObj.SetPlayerTransform(_player.transform);
            Debug.Log(botObj.GetPlayerTransform());
        }
    }

    [ContextMenu("Spawn Bot")]
    public void Spawn()
    {
        SpawnBot(bot, new Vector2(_player.transform.position.x + Random.Range(0.5f, 10), _player.transform.position.y + Random.Range(0.5f, 10)));
    }
}
