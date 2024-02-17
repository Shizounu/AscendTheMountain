using Shizounu.Library.ScriptableArchitecture;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Windows;

public class UIManager : MonoBehaviour
{
    [SerializeField, Header("Value References")] private ScriptableInt _playerMaxHealth;
    [SerializeField] private ScriptableInt _currentPlayerHealth;
    [SerializeField] private ScriptableInt _playerMoney;

    [SerializeField, Header("Object References")] private HealthBar _healthBar;
    [SerializeField] private TextMeshProUGUI _playerMoneyText;

    public Action<ScriptableInt> OnPlayerHealthChanged;
    public Action<ScriptableInt> OnPlayerMoneyChanged;

    private void OnEnable()
    {
        SetInititalValues();

        OnPlayerMoneyChanged += UpdateMoneyText;
        OnPlayerHealthChanged += UpdateHealthbar;
    }


    private void SetInititalValues()
    {
        UpdateMoneyText(_playerMoney);
        _healthBar.SetMaxHealth(_playerMaxHealth.runtimeValue);
    }

    public void UpdateMoneyText(ScriptableInt value)
    {
        
        _playerMoneyText.text = "current money: " + value.runtimeValue.ToString();
    }

    public void UpdateHealthbar(ScriptableInt value)
    {
        _healthBar.SetCurrentHealth(value.runtimeValue);
    }

    public void InvokeMoneyEvent()
    {
        if (_playerMoney.runtimeValue <= 0) return;
        _playerMoney.runtimeValue -= 10;
        OnPlayerMoneyChanged.Invoke(_playerMoney);
    }

    public void InvokeHealthEvent()
    {
        _currentPlayerHealth.runtimeValue-= 10;
        OnPlayerHealthChanged.Invoke(_currentPlayerHealth);
    }
}
