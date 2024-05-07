using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

// Contains all the player controls! This script may also contain some controls for other objects, as I did kind of treat this script
// as an "all-in-one" kind of script, but for the most part different things have different scripts
// This is a lot tho
// Player positions for each scene is stored in the ScriptablePos folder, to create a new one right click in the assets folder, 
// press 'Create', and select StartingPosition at the top
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerControls : MonoBehaviour
{
    public static List<string> inventory = new List<string>();
    private Rigidbody2D rb;
    public static Animator animator;
    public Vector3 movement;
    public int currentHealth;
    public int maxHealth = 100;
    public int damage = 20;
    private int score = 0;
    private bool isDeathScreenInstantiated = false;
    private bool isPaused = false;
    public static PlayerControls instance;
    public GunFill gunFill;
    public HealthBar healthBar;
    public TMP_Text scoreText;
    public TMP_Text timerText;
    public float timer;
    public float movementSpeed = 4;
    public float sprintSpeedMultiplier = 3.0F;
    public float regSpeedMultiplier = 0.5f;
    private bool isTimerRunning = true;
    public static bool wasFogBreached = false;
    public static bool isUsingGreen = false;
    public GameObject GreenOozePrefab;
    public GameObject popUpPrefab;
    public GameObject deathScreenPrefab;
    public GameObject noMoreTimeScreenPrefab;
    public GameObject fogExitPrefab;
    public GameObject healthBarCanvas;
    public GameObject hotBarCanvas;
    public GameObject FirstDayObjectivePrefab;
    public GameObject TestMapObjectivePrefab;
    public GameObject EndGameScreen;
    public RatStartingPosSO ratStartingPosSO;
    public RatStartingPosSO FirstDayPosSO;
    public RatStartingPosSO MainMenuPosSO;
    public RatStartingPosSO TestMapPosSO;
    public RatStartingPosSO TutorialPosSO;
    private SpriteRenderer playerSpriteRenderer;
    private Color originalColor;
    public InventoryItemData ItemDataCheese;
    public InventoryItemData ItemDataAmmo;
    public InventoryItemData ItemDataCoin;
    private readonly IEnumerable<object> slotDictionary;
    private int previousGreenOozeCount = 0;


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
            return;
        }
    }

    void Start()
    {
        timer = 120;
        score = 0;
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Initial assignments
        DontDestroyOnLoad(healthBarCanvas);
        DontDestroyOnLoad(hotBarCanvas);
        DontDestroyOnLoad(gameObject);
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = playerSpriteRenderer.color;
        GunController.CurrentAmmo = 0;
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        inventory?.Clear();
        gunFill.SetAmmo(0);
        isUsingGreen = false;
        if (gunFill != null)
        {
            gunFill.ResetGun();
        }
        animator.SetBool("isDead", false);
        isDeathScreenInstantiated = false;
        Debug.Log("isDead: " + animator.GetBool("isDead"));
        healthBarCanvas.SetActive(true);
        PauseMenu.GameIsPaused = false;
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = playerSpriteRenderer.color;
        playerSpriteRenderer.color = originalColor;
        foreach (var display in FindObjectsOfType<InventoryDisplay>())
        {
            display.ClearAllSlots();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        animator = GetComponent<Animator>();
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        playerSpriteRenderer.color = originalColor;
        Debug.Log("isDead: " + animator.GetBool("isDead"));
        Cinemachine.CinemachineVirtualCamera virtualCamera = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
        if (virtualCamera != null)
        {
            virtualCamera.PreviousStateIsValid = false;
            virtualCamera.Follow = gameObject.transform;
        }

        // Scene-specific logic
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            PauseMenu.GameIsPaused = false;
            healthBarCanvas.SetActive(true);
            inventory?.Clear();
            if (gunFill != null)
            {
                gunFill.ResetGun();
            }
            else
            {
                Debug.LogError("gun fill is null!");
            }
            healthBar.SetMaxHealth(maxHealth);
            ResetTimerAndScore();

            if (SceneManager.GetActiveScene().name == "Tutorial" && GameObject.FindGameObjectWithTag("HealthBarCanvas") == null)
            {
                Debug.Log("No health bar in the scene");
            }
            else
            {
                GameObject[] healthBarCanvasInstances = GameObject.FindGameObjectsWithTag("HealthBarCanvas");
                foreach (GameObject canvasInstance in healthBarCanvasInstances)
                {
                    if (!System.Object.ReferenceEquals(canvasInstance, healthBarCanvas))
                    {
                        Destroy(canvasInstance);
                    }
                }
            }
            DontDestroyOnLoad(healthBarCanvas);
        }
        else if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            healthBarCanvas.SetActive(false);
            SetStartingPosSOForMainMenu();
            gameObject.SetActive(false);
        }

        // Reset values for the new scene
        foreach (var display in FindObjectsOfType<InventoryDisplay>())
        {
            display.ClearAllSlots();
        }
        wasFogBreached = false;
        isUsingGreen = false;
        gameObject.SetActive(true);
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        animator.SetBool("isDead", false);
        isDeathScreenInstantiated = false;
        PauseMenu.GameIsPaused = false;

        // Load data for specific scenes
        if (SceneManager.GetActiveScene().name == "Tutorial")
        {
            ratStartingPosSO = TutorialPosSO;
        }
        else if (SceneManager.GetActiveScene().name == "1stDay")
        {
            ratStartingPosSO = FirstDayPosSO;
        }
        else if (SceneManager.GetActiveScene().name == "TestMap")
        {
            ratStartingPosSO = TestMapPosSO;
        }
        else
        {
            Debug.LogError("No starting position set for player");
        }

        if (ratStartingPosSO != null && ratStartingPosSO.startingPos != null)
        {
            gameObject.transform.position = ratStartingPosSO.startingPos;
        }
        else
        {
            Debug.LogError("Player position or starting position is not set!");
        }
    }


    private void SetStartingPosSOForMainMenu()
    {
        ratStartingPosSO = TutorialPosSO;
    }


    void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void Update()
    {
        int currentGreenOozeCount = inventory.Count(item => item == "GreenOoze");

        if (currentGreenOozeCount < previousGreenOozeCount)
        {
            Debug.Log("At least one instance of GreenOoze was removed from the inventory list.");
            var inventoryBar = transform.GetComponent<InventoryHolder>();
            bool removeFromInventory = inventoryBar.InventorySystem.RemoveFromInventory(ItemDataAmmo, 1);
        }

        previousGreenOozeCount = currentGreenOozeCount;

        if (isTimerRunning)
        {
            timer -= Time.deltaTime;
            UpdateTimerText();
            if (timer <= 0)
            {
                timer = 0;
                isTimerRunning = false;
                Time.timeScale = 0;
                InstantiateNoTimeLeftScreen();
                if (healthBarCanvas != null)
                {
                    healthBarCanvas.SetActive(false);
                }
            }
        }

        if (animator.GetBool("isDead") && !isDeathScreenInstantiated)
        {
            Time.timeScale = 0;
            InstantiateDeathScreen();
            isDeathScreenInstantiated = true;
            healthBarCanvas.SetActive(false);
        }

        if (GameObject.FindGameObjectsWithTag("Spawner").Length == 0)
        {
            Destroy(GameObject.FindGameObjectWithTag("FogExit"));
        }

        Move();
        Animate();
        RotateWeapon();
    }

    public void ResetTimerAndScore()
    {
        timer = 120;
        score = 0;
        UpdateScoreText();
        isTimerRunning = true;
    }

    public void UpdateTimerText()
    {
        if (timerText != null)
        {
            timerText.text = "Timer: " + Mathf.RoundToInt(timer).ToString();
        }
        else
        {
            Debug.LogError("Timer Text reference is not set in PlayerControls script!");
        }
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

    public void AddTime(float timeToAdd)
    {
        timer += timeToAdd;
        UpdateTimerText();
    }

    public void IncreaseScore(int amount)
    {
        score += amount;
        UpdateScoreText();
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
        else
        {
            animator.SetBool("isDead", false);
        }
    }

    IEnumerator FlashRed()
    {
        SpriteRenderer playerSpriteRenderer = GetComponent<SpriteRenderer>();
        Color originalColor = playerSpriteRenderer.color;
        playerSpriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        playerSpriteRenderer.color = originalColor;
        if (animator.GetBool("isDead"))
        {
            playerSpriteRenderer.color = originalColor;
        }
    }

    void Animate()
    {
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
    }

    void Move()
    {
        if (!isPaused)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            bool isSprinting = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.JoystickButton4));
            float currentSpeed = isSprinting ? movementSpeed * 3 : movementSpeed * 2;
            movement = new Vector3(horizontalInput, verticalInput, 0);
            transform.position += movement * Time.deltaTime * currentSpeed;
        }
    }

    public void ResetMovement()
    {
        animator.SetBool("isDead", false);
        rb.constraints = RigidbodyConstraints2D.None;
        rb.velocity = Vector2.zero;
        transform.position = new Vector3(0, 0, 0);
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Coin"))
        {
            string itemName = collision.gameObject.tag;
            inventory.Add(itemName);
            var inventoryBar = transform.GetComponent<InventoryHolder>();
            if (!inventoryBar)
            {
                Debug.LogError("No Inventory Holder");
            }
            bool addedToInventory = inventoryBar.InventorySystem.AddToInventory(ItemDataCoin, 1);
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
            isUsingGreen = true;
            gunFill.SetMaxAmmo(GunController.MaxAmmo);
            GunController.CurrentAmmo = GunController.MaxAmmo;
            string itemName = collision.gameObject.tag;
            inventory.Add(itemName);
            var inventoryBar = transform.GetComponent<InventoryHolder>();
            if (!inventoryBar)
            {
                Debug.LogError("No Inventory Holder");
            }
            bool addedToInventory = inventoryBar.InventorySystem.AddToInventory(ItemDataAmmo, 1);
            Destroy(collision.gameObject);
            if (inventory.Count(item => item == "GreenOoze") == 3)
            {
                if (SceneManager.GetActiveScene().name == "Tutorial")
                {
                    InstantiateFogExit();
                }
                else if (SceneManager.GetActiveScene().name == "1stDay")
                {
                    InstantiateFogExit();
                }
                else if (SceneManager.GetActiveScene().name == "TestMap")
                {
                    InstantiateFogExit();
                }
            }
        }
        else if (collision.gameObject.CompareTag("ChestClose") && inventory.Contains("Coin") && !inventory.Contains("GreenOoze"))
        {
            inventory.Remove("Coin");
            var inventoryBar = transform.GetComponent<InventoryHolder>();
            bool removeFromInventory = inventoryBar.InventorySystem.RemoveFromInventory(ItemDataCoin, 1);
            SpawnGreenOoze();
        }
        else if (collision.gameObject.CompareTag("Door1stDay"))
        {
            GameObject[] spawners = GameObject.FindGameObjectsWithTag("Spawner");
            if (spawners.Length == 0)
            {
                InstantiateObjScreen();
            }
            else
            {
                Debug.Log("Cannot destroy the door. Active spawners still exist.");
            }
        }
        else if (collision.gameObject.CompareTag("Cheese"))
        {
            string itemName = collision.gameObject.tag;
            inventory.Add(itemName);
            var inventoryBar = transform.GetComponent<InventoryHolder>();
            if (!inventoryBar)
            {
                Debug.LogError("No Inventory Holder");
            }
            bool addedToInventory = inventoryBar.InventorySystem.AddToInventory(ItemDataCheese, 1);

            IncreaseScore(20);
            UpdateScoreText();
            Destroy(collision.gameObject);

        }

        else if (collision.gameObject.CompareTag("CheeseDoor"))
        {
            int coinCount = inventory.Count(item => item == "Coin");
            if (coinCount >= 5)
            {
                Destroy(collision.gameObject);
                var inventoryBar = transform.GetComponent<InventoryHolder>();
                bool removeFromInventory = inventoryBar.InventorySystem.RemoveFromInventory(ItemDataCoin, 5);
            }
            else
            {
                Debug.Log("You need at least 5 Coins to open this door!");
            }
        }
        else if (collision.gameObject.CompareTag("ClockDoor"))
        {
            if (inventory.Contains("Cheese"))
            {
                Destroy(collision.gameObject);
                var inventoryBar = transform.GetComponent<InventoryHolder>();
                bool removeFromInventory = inventoryBar.InventorySystem.RemoveFromInventory(ItemDataCheese, 1);
            }
            else
            {
                Debug.Log("You need 1 Cheese to enter!");
            }
        }
        else if (collision.gameObject.CompareTag("Clock"))
        {
            AddTime(15);
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("FogEnter"))
        {
            wasFogBreached = true;
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("ToLevel1"))
        {
            InstantiateObjScreen();
        }
        else if (collision.gameObject.CompareTag("EndGame"))
        {
            InstantiateObjScreen();
        }
        else if (collision.gameObject.CompareTag("NoHealthHeart"))
        {
            TakeDamage(10);
            Destroy(collision.gameObject);
        }
    }

    void InstantiateFogExit()
    {
        if (SceneManager.GetActiveScene().name == "Tutorial")
        {
            Vector3 fogExitPosition = new Vector3(226.60f, 2.77f, 0.00f);
            GameObject fogExit = Instantiate(fogExitPrefab, fogExitPosition, Quaternion.identity);
        }
        else if (SceneManager.GetActiveScene().name == "1stDay")
        {
            Vector3 fogExitPosition = new Vector3(286.799988f, 77.9167023f, 0f);
            GameObject fogExit = Instantiate(fogExitPrefab, fogExitPosition, Quaternion.identity);
        }
        else if (SceneManager.GetActiveScene().name == "TestMap")
        {
            Vector3 fogExitPosition = new Vector3(221.824799f, 227.610001f, 0);
            Quaternion rotation = Quaternion.Euler(0, 0, 90);
            GameObject fogExit = Instantiate(fogExitPrefab, fogExitPosition, rotation);
        }
    }


    void SpawnGreenOoze()
    {
        if (GameObject.FindGameObjectWithTag("GreenOoze") != null)
        {
            return;
        }

        float spawnDistanceToLeft = 5.0f;

        for (int i = 3; i > 0; i--)
        {
            Vector3 playerPosition = transform.position;
            Vector3 spawnPoint = new Vector3(playerPosition.x - spawnDistanceToLeft, playerPosition.y, playerPosition.z);

            RaycastHit2D hit = Physics2D.Raycast(spawnPoint, Vector2.zero);

            while (hit.collider != null)
            {
                spawnPoint = new Vector3(spawnPoint.x + 1.0f, spawnPoint.y, spawnPoint.z);
                hit = Physics2D.Raycast(spawnPoint, Vector2.zero);
            }

            GameObject greenOoze = Instantiate(GreenOozePrefab, spawnPoint, Quaternion.identity);
            greenOoze.tag = "GreenOoze";
            Renderer oozeRenderer = greenOoze.GetComponent<Renderer>();
            if (oozeRenderer != null)
            {
                oozeRenderer.sortingLayerName = "Default";
                oozeRenderer.sortingOrder = 99;
            }
        }
    }


    void RotateWeapon()
    {
        if (!PauseMenu.GameIsPaused)
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

    public static void PrintInventory()
    {
        Debug.Log("INVENTORY:");
        foreach (string item in inventory)
        {
            Debug.Log("- " + item);
        }
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

    void InstantiateNoTimeLeftScreen()
    {
        if (noMoreTimeScreenPrefab != null)
        {
            Instantiate(noMoreTimeScreenPrefab, Vector3.zero, Quaternion.identity);
        }
        else
        {
            Debug.LogError("No more time screen prefab is not assigned!");
        }
    }

    void InstantiateObjScreen()
    {
        GameObject objScreenPrefabToInstantiate = null;

        if (SceneManager.GetActiveScene().name == "Tutorial")
        {
            objScreenPrefabToInstantiate = FirstDayObjectivePrefab;
        }
        else if (SceneManager.GetActiveScene().name == "1stDay")
        {
            objScreenPrefabToInstantiate = TestMapObjectivePrefab;
        }
        else if (SceneManager.GetActiveScene().name == "TestMap")
        {
            objScreenPrefabToInstantiate = EndGameScreen;
        }

        if (objScreenPrefabToInstantiate != null)
        {
            Instantiate(objScreenPrefabToInstantiate, Vector3.zero, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Objective screen prefab is not assigned!");
        }
    }

}
