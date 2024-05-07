using UnityEngine;

// Controls for enemy, let's the enemy chase you 
// It cannot recognize/go around walls, so in level design it's best if enemy spawners are in open spaces, or the enemies are made
// to be able to recognize/go around walls 
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    public float movementSpeed = 2.0f;
    private Transform playerTransform;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector3 movement;
    public int currentHealth;
    public int maxHealth = 300;
    public HealthBar enemyHealthBar;

    private PlayerControls playerControls;

    public void Initialize(PlayerControls playerControls)
    {
        this.playerControls = playerControls;
    }

    void Start()
    {
        currentHealth = maxHealth;
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
        enemyHealthBar.SetHealth(currentHealth);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();

            GameObject playerControlsObject = GameObject.FindGameObjectWithTag("Player");
            if (playerControlsObject != null)
            {
                PlayerControls playerControls = playerControlsObject.GetComponent<PlayerControls>();
                if (playerControls != null)
                {
                    playerControls.IncreaseScore(10);
                }
                else
                {
                    Debug.Log("PlayerControls component not found on player GameObject.");
                }
            }
            else
            {
                Debug.Log("Player GameObject not found in the scene.");
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
