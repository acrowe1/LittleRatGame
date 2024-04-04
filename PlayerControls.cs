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
    public GameObject Gun;
    public GameObject Bullet; // Reference to the bullet prefab
    public float bulletSpeed = 10f; // Speed of the bullet
    public GameObject chest;
    public TextMeshProUGUI chestText;
    public float coinSpawnRadius = 5.0F;
    private int totalCoins;
    private GameObject gunInstance;
    private bool isNearChest = false;
    private bool gunSpawned = false;

    public const int maxHealth = 100;
    public int currentHealth;

    public HealthBar HealthBar; // Reference to HealthBar script
    public StaminaBar StaminaBar; // Reference to StaminaBar script

    void Start()
    {
        currentHealth = maxHealth;
        HealthBar.SetMaxHealth(maxHealth); // Set max health in HealthBar script
        StaminaBar.SetMaxStamina(StaminaBar.maxStamina); // Set max stamina in StaminaBar script

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

            bool isSprinting = Input.GetKey(KeyCode.LeftShift) && StaminaBar.currentStamina > 0; // Check if sprinting and enough stamina
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

            // Shoot bullets if gun is attached and "Y" key is pressed
            if (Input.GetKeyDown(KeyCode.Y) || Input.GetKeyDown(KeyCode.JoystickButton3))
            {
                if (gunInstance != null)
                {
                    ShootBullet();
                }
            }

            // Example of taking damage (can be called from other methods)
            if (Input.GetKeyDown(KeyCode.Space)) // Example key to take damage
            {
                TakeDamage(10); // Example damage amount
            }
        }
    }

  void ShootBullet()
    {
        GameObject bullet = Instantiate(Bullet, gunInstance.transform.position, Quaternion.identity);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.velocity = gunInstance.transform.right * bulletSpeed;

        // Set bullet's gravity scale to zero
        bulletRb.gravityScale = 0f;

        // Set bullet's collision detection to Continuous
        bulletRb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        // Ignore collision between bullet and player
        Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        // Set bullet lifetime
        Destroy(bullet, 2.0f); // Bullet will be destroyed after 2 seconds
    }




    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        HealthBar.SetHealth(currentHealth); // Update health in HealthBar script
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
        else if (collision.gameObject.CompareTag("Gun") && gunSpawned == true)  // Check if gun has not been spawned yet
        {
            AttachGun();
        }
        else if (collision.gameObject.CompareTag("ChestClose") && !gunSpawned)  // Check if gun has not been spawned yet
        {
            Debug.Log("Collided with chest");
            if (inventory.Contains("Coin"))
            {
                Debug.Log("Spawning gun");
                SpawnGun();
                gunSpawned = true;  // Set gunSpawned flag to true
            }
            else
            {
                Debug.Log("No coin in inventory");
            }
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(20);
        }
    }

    void SpawnGun()
    {
        Vector2 randomSpawnPoint = Random.insideUnitCircle * coinSpawnRadius;
        Vector3 spawnPoint = new Vector3(transform.position.x + randomSpawnPoint.x, transform.position.y + randomSpawnPoint.y, -1);

        gunInstance = Instantiate(Gun, spawnPoint, Quaternion.identity);
        gunInstance.tag = "Gun";

        Renderer gunRenderer = gunInstance.GetComponent<Renderer>();
        if (gunRenderer != null)
        {
            gunRenderer.sortingLayerName = "Default";
            gunRenderer.sortingOrder = 1;
        }
    }

    void AttachGun()
    {
        if (gunInstance == null) return;

        gunInstance.transform.SetParent(this.transform);

        Vector3 gunPosition = new Vector3(0, 0.5f, 0);
        gunInstance.transform.localPosition = gunPosition;

        gunInstance.SetActive(true);
    }

    void PrintInventory()
    {
        Debug.Log("Inventory:");
        foreach (string item in inventory)
        {
            Debug.Log("- " + item);
        }
    }
}
