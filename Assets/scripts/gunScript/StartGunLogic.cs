using System.Collections.Generic;
using UnityEngine;

public class StartGunLogic : MonoBehaviour
{
    [SerializeField] private List<GameObject> gunPrefabs;
    private PlayerData _playerData;
    [SerializeField] private GameObject _player; 
    private int counter = 1;

    private void Start()
    {
        _playerData = _player.GetComponent<PlayerData>();
        
        GameObject pistol = gunPrefabs[0];
        GameObject obj = Instantiate(pistol, transform.position, Quaternion.identity);
        obj.transform.SetParent(transform);
    }

    void Update()
    {
        if (_playerData._score >= 100 && counter != gunPrefabs.Count)
        {
            GameObject obj = Instantiate(gunPrefabs[counter], transform.position, Quaternion.identity);
            obj.transform.SetParent(transform);
            counter++;
            _playerData.newLevel();
        }
    }
}