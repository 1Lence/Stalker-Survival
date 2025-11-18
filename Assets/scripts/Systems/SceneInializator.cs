using UnityEngine;
using UnityEngine.TextCore.Text;

public class SceneInializator : MonoBehaviour
{
    public static SceneInializator Instance {get; private set;}
    
    public System.Action OnGameStart;
    
    private PlayerControl playerControl;
    private PlayerData playerData;
    private SpawnSystem spawnSystem;
    private ScoreCounter scoreCounter;
    private PauseManager pauseManager;
    private GameFinisher gameFinisher;
    private ScoreUI scoreUI;
    private TimerUI timerUI;
    private HealthBar healthBar;

    private void Awake()
    {
        // Реализация синглтона
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        playerControl = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerControl>();
        playerData = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerData>();
        spawnSystem = FindObjectOfType<SpawnSystem>();
        scoreCounter = FindObjectOfType<ScoreCounter>();
        pauseManager = PauseManager.Instance;
        gameFinisher = FindObjectOfType<GameFinisher>();
        
        scoreUI = FindObjectOfType<ScoreUI>();
        timerUI = FindObjectOfType<TimerUI>();
        healthBar = FindObjectOfType<HealthBar>();

        if (playerControl == null) Debug.LogError("PlayerControl not found!");
        if (playerData == null) Debug.LogError("PlayerData not found!");
        if (spawnSystem == null) Debug.LogError("SpawnSystem not found!");
        if (scoreCounter == null) Debug.LogError("ScoreCounter not found!");
        if (pauseManager == null) Debug.LogError("PauseManager not found!");
        if (scoreUI == null) Debug.LogError("ScoreUI not found!");
        if (timerUI == null) Debug.LogError("TimerUI not found!");
        if (gameFinisher == null) Debug.LogError("GameFinisher not found!");
        
        if (playerControl == null || spawnSystem == null || scoreCounter == null 
            || scoreUI == null || timerUI == null || gameFinisher == null)
        {
            Debug.Log("Initialization failed due to missing components.");
            return;
        }

        AudioSystem.Instance?.PlayGameMusic();
        
        spawnSystem.OnScoreChanged += scoreCounter.UpdateScore;
        OnGameStart += scoreCounter.GameStart;
        
        scoreCounter.OnScoreChanged += playerData.UpdateScore;
        scoreCounter.OnScoreChanged += scoreUI.SetScore; 
        scoreCounter.OnTimeChanged += timerUI.SetTime;

        playerControl.OnHealthChange += healthBar.OnBarChangeAmount;
        playerControl.OnDeath += gameFinisher.GameFinish;
        
        //финиш игры
        gameFinisher.OnGameFinish += scoreCounter.GameFinished;
        gameFinisher.OnGameFinish += spawnSystem.StopSpawning;

        scoreCounter.OnGameFinishedScoreAndTime += pauseManager.GameFinishUI;

        pauseManager.OnGameQuit += UnsubscribeAll;
        
        
        OnGameStart?.Invoke();
    }

    public void UnsubscribeAll()
    {
        spawnSystem.OnScoreChanged -= scoreCounter.UpdateScore;
        OnGameStart -= scoreCounter.GameStart;
        
        scoreCounter.OnScoreChanged -= playerData.UpdateScore;
        scoreCounter.OnScoreChanged -= scoreUI.SetScore; 
        scoreCounter.OnTimeChanged -= timerUI.SetTime;

        playerControl.OnHealthChange -= healthBar.OnBarChangeAmount;
        playerControl.OnDeath -= gameFinisher.GameFinish;
        
        //финиш игры
        gameFinisher.OnGameFinish -= scoreCounter.GameFinished;
        gameFinisher.OnGameFinish -= spawnSystem.StopSpawning;

        scoreCounter.OnGameFinishedScoreAndTime -= pauseManager.GameFinishUI;

        pauseManager.OnGameQuit -= UnsubscribeAll;
    }
}
