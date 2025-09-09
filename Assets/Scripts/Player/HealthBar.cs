using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider healthbarSlider;
    [SerializeField] private Image healthbarFill;
    [SerializeField] private Gradient healthGradient;
    [SerializeField] private TextMeshProUGUI healthText;

    /*public void SetMaxHealth(float health)
    {
        if (healthbarSlider)
        {
            healthbarSlider.maxValue = health;
            healthbarSlider.value = health;
        }

        if (healthGradient != null && healthbarFill != null)
        {
            healthbarFill.color = healthGradient.Evaluate(1);
        }
    }*/

    public void SetHealth(float currentHealth,float maxHealth)
    {
        if(healthbarSlider)
        {
            healthbarSlider.maxValue = maxHealth;
            healthbarSlider.value = currentHealth;
        }

        if (healthGradient != null && healthbarFill != null)
        {
            healthbarFill.color = healthGradient.Evaluate(healthbarSlider.normalizedValue);
        }

        if (healthText)
            healthText.text = currentHealth.ToString() + "/" + maxHealth.ToString();
        else
            Debug.LogError("No text.");
    }
}
