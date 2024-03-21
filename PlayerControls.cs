// only uses keyboard controlls atm (W,A,S,D)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerControls : MonoBehaviour
{
    private List<string> inventory = new List<string>();
    public float movementSpeed = 5.0F;
    public float jumpForce = 10.0F;
    private Rigidbody2D rb;
    private bool isPaused = false;
    private bool isGrounded = true; 
    public GameObject coinPrefab; 


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0; 
    }

    void Update()
    {
        // I'm working on a rough pause menu 
        if (!isPaused) 
        {
            if (Input.GetKey(KeyCode.A)) {
                transform.position += Vector3.left * Time.deltaTime * movementSpeed;
            } else if (Input.GetKey(KeyCode.D)) {
                transform.position += Vector3.right * Time.deltaTime * movementSpeed;
            } else if (Input.GetKey(KeyCode.W)) {
                transform.position += Vector3.up * Time.deltaTime * movementSpeed; 
            } else if (Input.GetKey(KeyCode.S)) {
                transform.position += Vector3.down * Time.deltaTime * movementSpeed; 
            }

        }
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Coin")) {
            // Add the item to the inventory list
            string itemName = collision.gameObject.name;
            inventory.Add(itemName);

            // Destroy item after picking it up
            Destroy(collision.gameObject);
        }
    }
}
