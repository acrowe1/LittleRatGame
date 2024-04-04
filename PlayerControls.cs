using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerControls : MonoBehaviour
{
    private List<string> inventory = new List<string>();
    public float movementSpeed = 5.0F;
    public float sprintSpeedMultiplier = 2.0F;
    private Rigidbody2D rb;
    private bool isPaused = false;
    public GameObject Coin;
    public GameObject GreenOozePrefab; // Reference to the GreenOoze prefab
    public GameObject chestClose;
    public float coinSpawnRadius = 5.0F;
    private int totalCoins;
    public GameObject gunFill; // Reference to the gun's fill UI element or sprite renderer

    public const int maxHealth = 100;
    public int currentHealth;

    public HealthBar HealthBar;
    public StaminaBar StaminaBar;

    void Start()
    {
        currentHealth = maxHealth;
        HealthBar.SetMaxHealth(maxHealth);
        StaminaBar.SetMaxStamina(StaminaBar.maxStamina);

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        List<Vector3> spawnedCoinPositions = new List<Vector3>();

        float randomNumber = Random.Range(0f, 10f);
        totalCoins = Mathf.RoundToInt(randomNumber);

        for (int i = 0; i < totalCoins; i++)
        {
            Vector2 randomSpawnPoint = Random.insideUnitCircle * coinSpawnRadius;
            Vector3 spawnPosition = new Vector3(transform.position.x + randomSpawnPoint.x, transform.position.y + randomSpawnPoint.y, 0);

            while (IsPositionOccupied(spawnPosition, spawnedCoinPositions))
            {
                randomSpawnPoint = Random.insideUnitCircle * coinSpawnRadius;
                spawnPosition = new Vector3(transform.position.x + randomSpawnPoint.x, transform.position.y + randomSpawnPoint.y, 0);
            }

            GameObject newCoin = Instantiate(Coin, spawnPosition, Quaternion.identity);
            newCoin.tag = "Coin";

            Renderer coinRenderer = newCoin.GetComponent<Renderer>();
            if (coinRenderer != null)
            {
                coinRenderer.sortingLayerName = "Default";
                coinRenderer.sortingOrder = 1;
            }

            spawnedCoinPositions.Add(spawnPosition);
        }
    }

    bool IsPositionOccupied(Vector3 position, List<Vector3> occupiedPositions)
    {
        foreach (Vector3 occupiedPosition in occupiedPositions)
        {
            if (Vector3.Distance(position, occupiedPosition) < 1.0f)
            {
                return true;
            }
        }
        return false;
    }

    void Update()
    {
        if (!isPaused)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            // Check for sprinting with keyboard or controller
            bool isSprinting = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.JoystickButton0)) && StaminaBar.currentStamina > 0;
            float currentSpeed = isSprinting ? movementSpeed * sprintSpeedMultiplier : movementSpeed;

            if (isSprinting)
            {
                StaminaBar.currentStamina -= StaminaBar.staminaDepletionRate * Time.deltaTime;
                StaminaBar.SetStamina(StaminaBar.currentStamina);
            }
            else if (StaminaBar.currentStamina < StaminaBar.maxStamina)
            {
                StaminaBar.currentStamina += StaminaBar.staminaRegenRate * Time.deltaTime;
                StaminaBar.SetStamina(StaminaBar.currentStamina);
            }

            Vector3 movement = new Vector3(horizontalInput, verticalInput, 0);
            transform.position += movement * Time.deltaTime * currentSpeed;
        }
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        HealthBar.SetHealth(currentHealth);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Coin"))
        {
            string itemName = collision.gameObject.tag;
            inventory.Add(itemName);
            PrintInventory();
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(20);
        }
        else if (collision.gameObject.CompareTag("GreenOoze"))
        {
            string itemName = collision.gameObject.tag; 
            inventory.Add(itemName); 
            PrintInventory(); 
            Destroy(collision.gameObject); 

            // Update the gun's fill color to green
            UpdateGunFillColor(Color.green);
        }
        else if (collision.gameObject.CompareTag("ChestClose") && inventory.Contains("Coin"))
        {
            Debug.Log("Collided with chest and have at least one coin in inventory");
            SpawnGreenOoze();
        }
    }

    void PrintInventory()
    {
        Debug.Log("Inventory:");
        foreach (string item in inventory)
        {
            Debug.Log("- " + item);
        }
    }

    void SpawnGreenOoze()
    {
        if(GameObject.FindGameObjectWithTag("GreenOoze") != null)
        {
            Debug.Log("GreenOoze already spawned!");
            return;
        }

        float spawnDistanceToLeft = 1.0f; 

        Vector3 playerPosition = transform.position; 
        Vector3 spawnPoint = new Vector3(playerPosition.x - spawnDistanceToLeft, playerPosition.y, -1);

        GameObject greenOoze = Instantiate(GreenOozePrefab, spawnPoint, Quaternion.identity);
        greenOoze.tag = "GreenOoze";

        Renderer oozeRenderer = greenOoze.GetComponent<Renderer>();
        if (oozeRenderer != null)
        {
            oozeRenderer.sortingLayerName = "Default";
            oozeRenderer.sortingOrder = 1;
        }
    }

    void UpdateGunFillColor(Color color)
    {
        if (gunFill != null)
        {
            // Check if the reference is pointing to a UI Image or Sprite Renderer
            if (gunFill.GetComponent<SpriteRenderer>() != null)
            {
                gunFill.GetComponent<SpriteRenderer>().color = color;
            }
            else if (gunFill.GetComponent<UnityEngine.UI.Image>() != null)
            {
                gunFill.GetComponent<UnityEngine.UI.Image>().color = color;
            }
        }
    }
}
