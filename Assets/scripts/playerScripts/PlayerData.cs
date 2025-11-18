using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public int score;
    
    void Start()
    {

    }
    
    void Update()
    {

    }

    public void UpdateScore(int newScore)
    {
        score = newScore;
    }

    public int GetScore() => score;

    public void newLevel()
    {
        score = 0;
    }
}