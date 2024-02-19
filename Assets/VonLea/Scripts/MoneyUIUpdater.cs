using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shizounu.Library.ScriptableArchitecture;
using TMPro;

public class MoneyUIUpdater : MonoBehaviour, IScriptableEventListener
{
    [SerializeField, Header("References")] private ScriptableInt _playerMoney;
    [SerializeField] private ScriptableEvent _onMoneyChanged;
    [SerializeField] private TextMeshProUGUI _playerMoneyText;

    private void OnEnable()
    {
        _playerMoneyText.text = _playerMoney.runtimeValue.ToString();

        _onMoneyChanged += this;
    }

    public void EventResponse()
    {
        _playerMoneyText.text = _playerMoney.runtimeValue.ToString();
    }
}
