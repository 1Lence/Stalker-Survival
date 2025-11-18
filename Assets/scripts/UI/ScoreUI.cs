using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    private TextMeshProUGUI _tmp;
    
    void Awake()
    {
        _tmp = GetComponent<TextMeshProUGUI>();
        if (_tmp == null)
        {
            Debug.LogError("ScoreUI: TextMeshPro component not found!");
        }
    }


    ///Отображает переданное число в виде текста на UI-элементе.
    /// <param name="value">Число для отображения</param>
    public void SetScore(int value)
    {
        if (_tmp is not null)
        {
            _tmp.text = value.ToString();
        }
    }

    // Опционально: перегрузка для float (если понадобится)
    public void SetScore(float value)
    {
        if (_tmp != null)
        {
            _tmp.text = Mathf.RoundToInt(value).ToString();
        }
    }
}
