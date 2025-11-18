using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject teachingPanel;
    
    private const string IS_NEED_TEACH_KEY = "IsNeedTeach"; // Ключ для PlayerPrefs
    
    void Start()
    {
        // Проверяем, существует ли ключ "IsNeedTeach"
        if (!PlayerPrefs.HasKey(IS_NEED_TEACH_KEY))
        {
            // Если нет — создаём и устанавливаем значение 1 (true)
            PlayerPrefs.SetInt(IS_NEED_TEACH_KEY, 1);
            PlayerPrefs.Save(); // Сохраняем сразу (важно для WebGL)
        }
        
        mainPanel.SetActive(true);
        settingsPanel.SetActive(false);
        teachingPanel.SetActive(false);
        
        AudioSystem.Instance?.PlayMainMenuMusic();
        
    }

    public void ToSettings()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(true);
        teachingPanel.SetActive(false);
    }

    public void ToMainPanel()
    {
        mainPanel.SetActive(true);
        settingsPanel.SetActive(false);
        teachingPanel.SetActive(false);
    }

    public void StartGameMain()
    {
        int isNeedTeach = PlayerPrefs.GetInt(IS_NEED_TEACH_KEY);
        if (isNeedTeach == 1)
        {
            mainPanel.SetActive(false);
            settingsPanel.SetActive(false);
            teachingPanel.SetActive(true);
        }
        else
        {
            SceneManager.LoadScene("BotTest");
        }
    }

    public void StartGameTeach()
    {
        teachingPanel.SetActive(false);
        mainPanel.SetActive(false);
        settingsPanel.SetActive(false);
        PlayerPrefs.SetInt(IS_NEED_TEACH_KEY, 0);
        PlayerPrefs.Save();
        SceneManager.LoadScene("BotTest");
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
}
