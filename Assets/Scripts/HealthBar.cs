using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class HealthBar : MonoBehaviour
{
    private Damageable playerDamageable;

    public Slider healthSlider;
    public TMP_Text healthBarText;

    public void Initialize(Damageable damageable)
    {
        playerDamageable = damageable;

        // Initialize health bar values
        healthSlider.value = CalculateSliderPercentage(playerDamageable.Health, playerDamageable.MaxHealth);
        healthBarText.text = "HP " + playerDamageable.Health + " / " + playerDamageable.MaxHealth;

        // Subscribe to health change events
        playerDamageable.healthChanged.AddListener(OnPlayerHealthChanged);
    }

    private void OnEnable()
    {
        if (playerDamageable != null)
        {
            playerDamageable.healthChanged.AddListener(OnPlayerHealthChanged);
        }
    }

    private void OnDisable()
    {
        if (playerDamageable != null)
        {
            playerDamageable.healthChanged.RemoveListener(OnPlayerHealthChanged);
        }
    }

    private float CalculateSliderPercentage(float currentHealth, float maxHealth)
    {
        return currentHealth / maxHealth;
    }

    private void OnPlayerHealthChanged(float newHealth, float maxHealth)
    {
        healthSlider.value = CalculateSliderPercentage(newHealth, maxHealth);
        healthBarText.text = "HP " + newHealth + " / " + maxHealth;
    }
}
