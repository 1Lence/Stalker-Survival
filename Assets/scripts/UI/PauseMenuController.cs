using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }
    
    [SerializeField] private GameObject pauseMenuUI; // Ссылка на объект меню паузы в инспекторе
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private TextMeshProUGUI tmp;
    [SerializeField] private GameObject gameFinishUi;
    
    private bool isPaused = false;
    
    public System.Action OnGameQuit;

    void Awake()
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
        gameFinishUi.SetActive(false);
        
        if (pauseMenuUI is not null)
            pauseMenuUI.SetActive(false); // Убедимся, что меню изначально скрыто
        else
            Debug.LogWarning("Pause menu UI не назначен в инспекторе!");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    // Публичный метод для возобновления игры
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        settingsPanel.SetActive(false);
        
        Time.timeScale = 1f;
        isPaused = false;
    }

    // Публичный метод для паузы
    public void Pause()
    {
        if (pauseMenuUI is not null)
            pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ToSettings()
    {
        pauseMenuUI.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void ToPauseMenu()
    {
        pauseMenuUI.SetActive(true);
        settingsPanel.SetActive(false);
    }

    public void GameFinishUI(int score, int time)
    {
        Time.timeScale = 0f;
        gameFinishUi.SetActive(true);
        tmp.text = score.ToString();
    }

    public void Quit()
    {
        OnGameQuit?.Invoke();
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main");
    }

    // Публичный метод для проверки состояния паузы (опционально, но полезно)
    public bool IsPaused() => isPaused;
}