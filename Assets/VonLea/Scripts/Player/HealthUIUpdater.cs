using Shizounu.Library.ScriptableArchitecture;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthUIUpdater : MonoBehaviour, IScriptableEventListener
{
    [SerializeField, Header("References")] private ScriptableInt _playerMaxHealth;
    [SerializeField] private ScriptableInt _playerCurrentHealth;
    [SerializeField] private ScriptableEvent _onHealthChanged;
    [SerializeField] private HealthBar _healthBar;

    private void OnEnable()
    {
        _healthBar.SetMaxHealth(_playerMaxHealth.runtimeValue);
        _healthBar.SetCurrentHealth(_playerCurrentHealth.runtimeValue);

        _onHealthChanged += this;
    }

    private void OnDisable()
    {
        _onHealthChanged -= this;
    }

    public void EventResponse()
    {
        if (_playerCurrentHealth.runtimeValue <= 0)
        {
            SceneManager.LoadScene(6); // loads death scene which is index 6 in build settings
            return;
        }

        _healthBar.SetCurrentHealth(_playerCurrentHealth.runtimeValue);
    }

}
