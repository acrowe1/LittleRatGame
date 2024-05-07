using UnityEngine;

// Code for coin spawner borders (usually named CoinBorder)
// To make a new coin spawner, a.k.a, a new border to spawn coins within, make a rectangle and attack this script to it, 
// it will automatically spawn coins in it randomly in said border
public class CoinSpawner : MonoBehaviour
{
    public GameObject coinPrefab;
    public int numberOfCoins = 10;
    public float innerBorderMargin = 1.0f;

    private void Start()
    {
        SpawnCoins();
    }

    void SpawnCoins()
    {
        Bounds innerBorderBounds = CalculateInnerBorderBounds();

        float constrainedMinX = innerBorderBounds.min.x + innerBorderMargin;
        float constrainedMaxX = innerBorderBounds.max.x - innerBorderMargin;
        float constrainedMinY = innerBorderBounds.min.y + innerBorderMargin;
        float constrainedMaxY = innerBorderBounds.max.y - innerBorderMargin;

        for (int i = 0; i < numberOfCoins; i++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(constrainedMinX, constrainedMaxX),
                Random.Range(constrainedMinY, constrainedMaxY),
                0
            );

            Instantiate(coinPrefab, randomPosition, Quaternion.identity);
        }
    }

    Bounds CalculateInnerBorderBounds()
    {
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();

        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minY = float.MaxValue;
        float maxY = float.MinValue;

        foreach (Collider2D collider in colliders)
        {
            Vector2 colliderMin = collider.bounds.min;
            Vector2 colliderMax = collider.bounds.max;

            if (colliderMin.x < minX) minX = colliderMin.x;
            if (colliderMax.x > maxX) maxX = colliderMax.x;
            if (colliderMin.y < minY) minY = colliderMin.y;
            if (colliderMax.y > maxY) maxY = colliderMax.y;
        }

        minX += innerBorderMargin;
        maxX -= innerBorderMargin;
        minY += innerBorderMargin;
        maxY -= innerBorderMargin;

        return new Bounds(new Vector3((minX + maxX) / 2, (minY + maxY) / 2, 0), new Vector3(maxX - minX, maxY - minY, 0));
    }
}
