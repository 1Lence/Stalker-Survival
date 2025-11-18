using UnityEngine;

public class AudioSystem : MonoBehaviour
{
    public static AudioSystem Instance { get; private set; }
    
    [Header("Music")] 
    [SerializeField] private AudioClip MainMenuTheme;
    [SerializeField] private AudioClip GameTheme;

    [Header("Player Weapon Sounds")] 
    [SerializeField] private AudioClip PlayerShootPistol;
    [SerializeField] private AudioClip PlayerShootAR;
    [SerializeField] private AudioClip PlayerShootShotgun;
    [SerializeField] private AudioClip PlayerShootSniper;

    [Header("Enemy Sounds")] 
    [SerializeField] private AudioClip MutantAtackBig;
    [SerializeField] private AudioClip MutantAtackSmall;
    [SerializeField] private AudioClip MutantShoot;

    // Ключи для PlayerPrefs
    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SFX_VOLUME_KEY = "SfxVolume";

    private float musicVolume = 0.5f;
    private float sfxVolume = 0.7f;

    private AudioSource musicSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;

        LoadVolumeSettings();
        musicSource.volume = musicVolume;
    }

    void Start()
    {
        PlayMainMenuMusic();
    }

    // === Музыка ===
    public void PlayMainMenuMusic()
    {
        if (MainMenuTheme != null)
        {
            musicSource.clip = MainMenuTheme;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning("MainMenuTheme не назначен!");
        }
    }

    public void PlayGameMusic()
    {
        if (GameTheme != null)
        {
            musicSource.clip = GameTheme;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning("GameTheme не назначен!");
        }
    }

    // === Громкость ===
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        musicSource.volume = musicVolume;
        SaveVolumeSettings();
    }

    public void SetSfxVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        SaveVolumeSettings();
    }

    public float GetMusicVolume() => musicVolume;
    public float GetSfxVolume() => sfxVolume;

    // === УНИВЕРСАЛЬНЫЙ МЕТОД ДЛЯ ЛЮБОГО ЗВУКА ===
    /// <summary>
    /// Проигрывает любой AudioClip как 2D-звук с текущей громкостью эффектов.
    /// </summary>
    public void PlayOneShot(AudioClip clip)
    {
        if (clip == null) return;

        GameObject tempAudio = new GameObject("TempAudio");
        AudioSource audioSource = tempAudio.AddComponent<AudioSource>();
        
        audioSource.clip = clip;
        audioSource.volume = sfxVolume;
        audioSource.spatialBlend = 0f; // 2D
        audioSource.Play();

        Destroy(tempAudio, clip.length);
    }

    // === Старые методы (можно оставить для удобства или удалить) ===
    public void PlayPistolShot() => PlayOneShot(PlayerShootPistol);
    public void PlayARShot() => PlayOneShot(PlayerShootAR);
    public void PlayShotgunShot() => PlayOneShot(PlayerShootShotgun);
    public void PlaySniperShot() => PlayOneShot(PlayerShootSniper);
    public void PlayMutantAtackBig() => PlayOneShot(MutantAtackBig);
    public void PlayMutantAtackSmall() => PlayOneShot(MutantAtackSmall);
    public void PlayMutantShot() => PlayOneShot(MutantShoot);

    // === Сохранение/загрузка ===
    private void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, musicVolume);
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, sfxVolume);
        PlayerPrefs.Save();
    }

    private void LoadVolumeSettings()
    {
        musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 0.5f);
        sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 0.7f);
    }
}
