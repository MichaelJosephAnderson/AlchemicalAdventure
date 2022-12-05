using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _logToSpawn;
    [SerializeField] private float _spawnRate;

    private float _timer;
    
    void Update()
    {
        if (_timer <= 0)
        {
            GameObject newLog = Instantiate(_logToSpawn);
            newLog.transform.position = transform.position;
            newLog.transform.parent = this.transform;
            
            _timer = _spawnRate;
        }
        else
        {
            _timer -= Time.deltaTime;
        }
    }
}
