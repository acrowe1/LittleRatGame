using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public Slider slider; 
    public float maxStamina = 100f; 
    public float currentStamina; 
    public float staminaDepletionRate = 10f; // Rate at which stamina depletes when sprinting
    public float staminaRegenRate = 5f; // Rate at which stamina regenerates when not sprinting

    private bool isSprinting = false; // Flag to check if player is sprinting
    private PlayerControls playerControls; // Reference to PlayerControls script

    void Start()
    {
        currentStamina = maxStamina;
        SetMaxStamina(maxStamina);

        // Get reference to PlayerControls script
        playerControls = GetComponent<PlayerControls>();
        if (playerControls == null)
        {
            Debug.LogError("PlayerControls script not found on the same GameObject as StaminaBar.");
        }
    }

    void Update()
    {
        bool isShiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.JoystickButton0);

        // Check if player is sprinting
        if (isShiftPressed)
        {
            if (currentStamina > 0)
            {
                isSprinting = true;
                currentStamina -= staminaDepletionRate * Time.deltaTime;
                SetStamina(currentStamina);
            }
            else
            {
                isSprinting = false;
            }
        }
        else
        {
            isSprinting = false;

            // Set player's speed to default speed if stamina is depleted
            if (currentStamina == 0 && playerControls != null)
            {
                playerControls.movementSpeed = playerControls.movementSpeed / playerControls.sprintSpeedMultiplier;
            }
        }

        // Regenerate stamina if not sprinting
        if (!isSprinting && currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            SetStamina(currentStamina);
        }
    }

    public void SetMaxStamina(float stamina)
    {
        slider.maxValue = stamina;
        slider.value = stamina;
    }

    public void SetStamina(float stamina)
    {
        slider.value = stamina;
    }
}
