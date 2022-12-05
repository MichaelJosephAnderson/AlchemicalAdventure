using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using TMPro;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private Collider _collider;
    [SerializeField] private TMP_Text _countdownText;
    [SerializeField] private int _numtoCountFrom;

    public static event Action OnLevelEnter;

    private bool _startCountdown;
    private bool _loadedLevel;
    private float _timer;

    private void OnEnable()
    {
        OnLevelEnter += DisableCollider;
    }

    private void OnDisable()
    {
        OnLevelEnter -= DisableCollider;
    }

    private void Start()
    {
        _countdownText.enabled = false;
    }

    private void Update()
    {
        if (_startCountdown)
        {
            if (_timer <= 0)
            {
                if (!_loadedLevel)
                {
                    ScenePicker scenePicker = GetComponent<ScenePicker>();
                    scenePicker.LoadScene();
                    _loadedLevel = true;
                }

            }
            else
            {
                _timer -= Time.deltaTime;
                int round = Mathf.RoundToInt(_timer);
                _countdownText.text = "Sending Players to Level in: " + round;
            }
        }
    }

    private void StartCountdown()
    {
        _countdownText.text = "Sending Players to Level in: " + _numtoCountFrom;
        _countdownText.enabled = true;
        _timer = _numtoCountFrom;
        
        _startCountdown = true;
    }

    private void DisableCollider()
    {
        _collider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCountdown();
            OnLevelEnter?.Invoke();
        }
    }
}
