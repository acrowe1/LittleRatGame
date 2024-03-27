using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerControls : MonoBehaviour
{
    private List<string> inventory = new List<string>();
    public float movementSpeed = 5.0F;
    public float sprintSpeedMultiplier = 2.0F; // Multiplier for sprint speed
    public float jumpForce = 10.0F;
    private Rigidbody2D rb;
    private bool isPaused = false;
    private bool isGrounded = true; 
    public GameObject Coin; 
    public float coinSpawnRadius = 5.0F; // Radius within which coins will spawn

void Start()
{
    rb = GetComponent<Rigidbody2D>();
    rb.gravityScale = 0;

    List<Vector3> spawnedCoinPositions = new List<Vector3>(); 

    float randomNumber = Random.Range(0f, 10f); 


    // Generate random coin spawn points within a radius around the player
    for (int i = 0; i < randomNumber; i++) 
    {
        Vector2 randomSpawnPoint = Random.insideUnitCircle * coinSpawnRadius;
        Vector3 spawnPosition = new Vector3(transform.position.x + randomSpawnPoint.x, transform.position.y + randomSpawnPoint.y, 0);

        // Check if the spawn position is already occupied by another coin
        while (IsPositionOccupied(spawnPosition, spawnedCoinPositions))
        {
            randomSpawnPoint = Random.insideUnitCircle * coinSpawnRadius;
            spawnPosition = new Vector3(transform.position.x + randomSpawnPoint.x, transform.position.y + randomSpawnPoint.y, 0);
        }

        GameObject newCoin = Instantiate(Coin, spawnPosition, Quaternion.identity);
        // Assign "Coin" tag to the instantiated coin object
        newCoin.tag = "Coin"; 
        spawnedCoinPositions.Add(spawnPosition); 
    }
}

// Method to check if a position is already occupied by another coin
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
        // I'm working on a rough pause menu 
        if (!isPaused) 
        {
            // Controller controls
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            // hold down left shift/'A' to sprint     
            bool isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.JoystickButton0); 
            float currentSpeed = isSprinting ? movementSpeed * sprintSpeedMultiplier : movementSpeed;

            Vector3 movement = new Vector3(horizontalInput, verticalInput, 0);
            transform.position += movement * Time.deltaTime * currentSpeed;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Coin")) 
        {
            // Add the item to the inventory list & destroy after
            string itemName = collision.gameObject.name;
            inventory.Add(itemName);
            PrintInventory();
            Destroy(collision.gameObject);
        }
    }

    // print inventory list to console
    void PrintInventory()
    {
        Debug.Log("Inventory:");
        foreach (string item in inventory)
        {
            Debug.Log("- " + item);
        }
    }
}
