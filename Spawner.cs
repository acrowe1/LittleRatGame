using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnRate = 2f;
    private float nextSpawnTime;
    private PlayerControls playerControls;

    void Start()
    {
        playerControls = FindObjectOfType<PlayerControls>();
    }

    void Update()
    {
        if (Time.time >= nextSpawnTime && PlayerControls.inventory.Contains("GreenOoze"))
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + 3f / spawnRate;
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
}
