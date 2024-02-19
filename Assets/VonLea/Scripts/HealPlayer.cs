using Shizounu.Library.ScriptableArchitecture;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealPlayer : MonoBehaviour
{
    [SerializeField] private ScriptableInt _playerMaxHealth;
    [SerializeField] private ScriptableInt _playerCurrentHealth;
    [SerializeField] private int _healPercentage;

    public void AddHealth()
    {
        if (!CanHeal()) return;

        Debug.Log("hp before healing: " +  _playerCurrentHealth.runtimeValue);
        int value = _healPercentage * _playerCurrentHealth.runtimeValue / 100;

        if(_playerCurrentHealth.runtimeValue + value > _playerMaxHealth.runtimeValue)
        {
            Debug.Log("reached healing limit");
            _playerCurrentHealth.runtimeValue = _playerMaxHealth.runtimeValue;
        }else _playerCurrentHealth.runtimeValue += value;

        Debug.Log("hp after healing: " + _playerCurrentHealth.runtimeValue);
    }

    private bool CanHeal()
    {
        if (_playerCurrentHealth.runtimeValue <= _playerMaxHealth.runtimeValue)
        {
            return true;
        }
        else return false;
    }
}
