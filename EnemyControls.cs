using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    public float movementSpeed = 10.0f;
    private Transform playerTransform;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector3 movement;
    public int currentHealth;
    public int maxHealth = 300;
    public HealthBar enemyHealthBar;

    private PlayerControls playerControls;  // Reference to the PlayerControls script

    public void Initialize(PlayerControls playerControls)
    {
        this.playerControls = playerControls;
    }

    void Start()
    {
        currentHealth = maxHealth;
        Debug.Log("Current health: " + currentHealth);
        enemyHealthBar.SetMaxHealth(maxHealth);

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        animator = GetComponent<Animator>();
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player not found! Make sure the player GameObject is tagged with 'Player'.");
        }
    }

    void Update()
    {
        if (playerTransform != null)
        {
            MoveTowardsPlayer();
            Animate();
        }
    }

    void Animate()
    {
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
    }

    void MoveTowardsPlayer()
    {
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        movement = new Vector3(directionToPlayer.x, directionToPlayer.y, 0);
        transform.position += directionToPlayer * movementSpeed * Time.deltaTime;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Current Health: " + currentHealth);
        enemyHealthBar.SetHealth(currentHealth);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();

            if (playerControls != null)
            {
                playerControls.IncreaseScore(10);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            TakeDamage(GunController.bulletDamage);
            Destroy(collision.gameObject);
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
