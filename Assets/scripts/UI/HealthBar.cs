using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HealthBar : MonoBehaviour
{
    public Slider slider;

    void Start()    {        slider.maxValue = PlayerController.maxHealth;        slider.value = PlayerController.currentHealth;    }

    public void SetHealth(float health)    {        slider.value = health;    }
}
