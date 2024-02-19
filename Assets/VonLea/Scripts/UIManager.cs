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
    
    public void SubstractMoney()
    {
        if (_playerMoney.runtimeValue <= 0) return;
        _playerMoney.runtimeValue -= 10;
        //OnPlayerMoneyChanged.Invoke(_playerMoney);
    }

    public void SubstractHealth()
    {
        if(_currentPlayerHealth.runtimeValue <= 0) return;
        _currentPlayerHealth.runtimeValue-= 10;
        //OnPlayerHealthChanged.Invoke(_currentPlayerHealth);
    }

}
