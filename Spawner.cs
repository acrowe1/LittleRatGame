using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

// Code for the enemy spawners, i.e., how many enemies spawn, the amount of time between waves, the health of the spawner
// itself 
public class Spawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnRate = 1f;
    private float nextSpawnTime;
    private PlayerControls playerControls;
    private int enemiesSpawned = 0;
    private readonly int totalEnemiesToSpawn = 5;
    private readonly float timeBetweenWaves = 30f;
    private float nextWaveTime;
    public int currentHealth;
    public int maxHealth = 1000;
    public HealthBar spawnerHealthBar;

    void Start()
    {
        currentHealth = maxHealth;
        spawnerHealthBar.SetMaxHealth(maxHealth);
        playerControls = FindObjectOfType<PlayerControls>();
        nextWaveTime = Time.time + timeBetweenWaves;
    }

    void Update()
    {
        if (Time.time >= nextSpawnTime && enemiesSpawned < totalEnemiesToSpawn && PlayerControls.wasFogBreached == true && PlayerControls.inventory.Count(item => item == "GreenOoze") == 3)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + 1f / spawnRate;
            enemiesSpawned++;
        }
        else if (Time.time >= nextWaveTime && enemiesSpawned >= totalEnemiesToSpawn)
        {
            nextWaveTime = Time.time + timeBetweenWaves;
            enemiesSpawned = 0;
        }
    }

    void SpawnEnemy()
    {
        GameObject enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        EnemyController enemyController = enemy.GetComponent<EnemyController>();
        if (enemyController != null)
        {
            enemyController.Initialize(playerControls);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        spawnerHealthBar.SetHealth(currentHealth);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
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
