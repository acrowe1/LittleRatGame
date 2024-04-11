
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.PlasticSCM.Editor.WebApi;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerControls : MonoBehaviour
{
    public static List<string> inventory = new List<string>();
    public float movementSpeed;
    public float sprintSpeedMultiplier = 2.0F;
    private Rigidbody2D rb;
    private bool isPaused = false;
    public GameObject Coin;
    public GameObject GreenOozePrefab;
    public GameObject chestClose;
    public float coinSpawnRadius = 5.0F;
    private int totalCoins;
    private bool greenOozeSpawned = false;
    public Animator animator;
    public Vector3 movement;
    public int currentHealth;
    public int maxHealth = 100;
    public HealthBar healthBar;
    public GameObject popUpPrefab;
    public int damage = 20;
    public GameObject deathScreenPrefab;
    private bool isDeathScreenInstantiated;
    public static PlayerControls instance;
    public TMP_Text scoreText;
    private int score = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        animator = GetComponent<Animator>();

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
                coinRenderer.sortingOrder = 99;
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(currentHealth);
        }
        if (animator.GetBool("isDead") && !isDeathScreenInstantiated)
        {
            Time.timeScale = 0;
            InstantiateDeathScreen();
            isDeathScreenInstantiated = true;
        }

        Move();
        Animate();
        RotateWeapon();
    }

    void InstantiateDeathScreen()
    {
        if (deathScreenPrefab != null)
        {
            Instantiate(deathScreenPrefab, Vector3.zero, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Death screen prefab is not assigned!");
        }
    }

    void TakeDamage(int damage)
    {
        damage = this.damage;
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
        if (currentHealth <= 0)
        {
            animator.SetBool("isDead", true);
        }
    }

    public void ResetMovement()
    {
        animator.SetBool("isDead", false);
        rb.constraints = RigidbodyConstraints2D.None;
        rb.velocity = Vector2.zero;
        transform.position = new Vector3(0, 0, 0);
    }

    void Animate()
    {
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
    }

    public void IncreaseScore(int amount)
    {
        score += amount;
        UpdateScoreText();
    }

    public void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
        else
        {
            Debug.LogError("Score Text reference is not set in PlayerControls script!");
        }
    }

    void Move()
    {
        if (!isPaused)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            bool isSprinting = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.JoystickButton5));
            float currentSpeed = isSprinting ? movementSpeed * sprintSpeedMultiplier : movementSpeed;

            movement = new Vector3(horizontalInput, verticalInput, 0);
            transform.position += movement * Time.deltaTime * currentSpeed;

        }
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
            Vector3 popUpPosition = this.transform.position + new Vector3(0, 1, 0);
            GameObject popUp = Instantiate(popUpPrefab, popUpPosition, Quaternion.identity);
            popUp.GetComponentInChildren<TMP_Text>().text = damage.ToString();
            TakeDamage(20);

            StartCoroutine(FlashRed());
        }
        else if (collision.gameObject.CompareTag("GreenOoze"))
        {
            string itemName = collision.gameObject.tag;
            inventory.Add(itemName);
            PrintInventory();
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("ChestClose") && inventory.Contains("Coin") && !greenOozeSpawned)
        {
            Debug.Log("Collided with chest and have at least one coin in inventory");
            SpawnGreenOoze();
            greenOozeSpawned = true;
        }
        else if (collision.gameObject.CompareTag("Door") && score >= 300)
        {
            Destroy(collision.gameObject);
        }
    }

    IEnumerator FlashRed()
    {
        SpriteRenderer playerSpriteRenderer = GetComponent<SpriteRenderer>();
        Color originalColor = playerSpriteRenderer.color;

        playerSpriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        playerSpriteRenderer.color = originalColor;
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
        if (GameObject.FindGameObjectWithTag("GreenOoze") != null)
        {
            Debug.Log("GreenOoze already spawned!");
            return;
        }

        float spawnDistanceToLeft = 5.0f;

        Vector3 playerPosition = transform.position;
        Vector3 spawnPoint = new Vector3(playerPosition.x - spawnDistanceToLeft, playerPosition.y, playerPosition.z);

        GameObject greenOoze = Instantiate(GreenOozePrefab, spawnPoint, Quaternion.identity);
        greenOoze.tag = "GreenOoze";

        Renderer oozeRenderer = greenOoze.GetComponent<Renderer>();
        if (oozeRenderer != null)
        {
            oozeRenderer.sortingLayerName = "Default";
            oozeRenderer.sortingOrder = 1;
        }
    }

    void RotateWeapon()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mouseDirection = (mousePosition - transform.position).normalized;

        float horizontalInput = Input.GetAxis("Mouse X");
        float verticalInput = Input.GetAxis("Mouse Y");

        Vector2 joystickDirection = new Vector2(horizontalInput, verticalInput).normalized;
        Vector2 direction = joystickDirection.magnitude > 0.1f ? joystickDirection : mouseDirection;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        GameObject gun = GameObject.FindGameObjectWithTag("Gun");
        if (gun != null)
        {
            gun.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

}