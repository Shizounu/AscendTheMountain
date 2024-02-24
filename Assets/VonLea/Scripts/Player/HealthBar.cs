using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private Gradient _gradient;
    [SerializeField] private Image _fill;

    public void SetMaxHealth(int maxHealth)
    {
        _slider.maxValue = maxHealth;
        //_slider.value = maxHealth;

        //_fill.color = _gradient.Evaluate(1f);
    }

    public void SetCurrentHealth(int currentHealth)
    {
        _slider.value = currentHealth;

        _fill.color = _gradient.Evaluate(_slider.normalizedValue);
    }
}
