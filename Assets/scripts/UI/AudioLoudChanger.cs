using UnityEngine;
using UnityEngine.UI;

public class AudioLoudChanger : MonoBehaviour
{
    [SerializeField] private AudioType type;
    private Slider _slider;

    void Start()
    {
        _slider = GetComponent<Slider>();
        if (_slider is not null)
        {
            switch (type)
            {
                case AudioType.Music:
                    _slider.value = AudioSystem.Instance.GetMusicVolume();
                    break;
                case AudioType.SFX:
                    _slider.value = AudioSystem.Instance.GetSfxVolume();
                    break;
            }
        }
        _slider.onValueChanged.AddListener(ChangeAudioVolume);
    }

    void OnDestroy()
    {
        _slider.onValueChanged.RemoveListener(ChangeAudioVolume);
    }
    
    public void ChangeAudioVolume(float volume)
    {
        switch (type)
        {
            case AudioType.Music:
                AudioSystem.Instance?.SetMusicVolume(volume);
                break;
            case AudioType.SFX:
                AudioSystem.Instance?.SetSfxVolume(volume);
                break;
        }
    }
}

public enum AudioType
{
    Music,
    SFX
}
