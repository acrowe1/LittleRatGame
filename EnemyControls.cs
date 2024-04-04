using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))] // Automatically add a Rigidbody2D component if it doesn't exist
public class EnemyController : MonoBehaviour
{
    public float movementSpeed = 10.0f; // Movement speed of the enemy

    private Transform playerTransform; // Reference to the player's transform
    private Rigidbody2D rb; // Reference to the Rigidbody2D component

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        // Find the player's transform
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player not found! Make sure the player GameObject is tagged with 'Player'.");
        }

        // Get the Rigidbody2D component
        rb = GetComponent<Rigidbody2D>();

        // Set Rigidbody2D constraints to freeze rotation
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    void Update()
    {
        // Check if playerTransform is not null before moving
        if (playerTransform != null)
        {
            // Move towards the player
            MoveTowardsPlayer();
        }
    }

    void MoveTowardsPlayer()
    {
        // Calculate the direction from the enemy to the player
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;

        // Move the enemy in the direction of the player
        transform.position += directionToPlayer * movementSpeed * Time.deltaTime;
    }
}
