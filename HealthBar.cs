using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;

    public const int maxHealth = 100;
    public int currentHealth;
    private PlayerControls playerControls; // Reference to PlayerControls


    void Start()
    {
        currentHealth = maxHealth;
        SetMaxHealth(maxHealth);
    }

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    public void SetHealth(int health)
    {
        slider.value = health;
    }

public void TakeDamage(int damage)
{
    currentHealth -= damage;
    SetHealth(currentHealth);
    Debug.Log("HealthBar updated. Current Health: " + currentHealth); // Added debug statement
}

}
