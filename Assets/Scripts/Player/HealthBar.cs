using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider healthbarSlider;
    [SerializeField] private Image healthbarFill;
    [SerializeField] private Gradient healthGradient;
    public void SetMaxHealth(float health)
    {
        if(healthbarSlider)
        {
            healthbarSlider.maxValue = health;
            healthbarSlider.value = health;
        }

        if (healthGradient != null && healthbarFill != null)
        {
            healthbarFill.color = healthGradient.Evaluate(1);    
        }
    }

    public void SetHealth(float health)
    {
        if(healthbarSlider)
            healthbarSlider.value = health;

        if (healthGradient != null && healthbarFill != null)
        {
            healthbarFill.color = healthGradient.Evaluate(healthbarSlider.normalizedValue);
        }
    }
}
