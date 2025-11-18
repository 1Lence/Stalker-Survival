using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFinisher : MonoBehaviour
{
    public System.Action OnGameFinish;
    void Awake()
    {

    }

    public void GameFinish()
    {
        OnGameFinish?.Invoke();
    }
}
