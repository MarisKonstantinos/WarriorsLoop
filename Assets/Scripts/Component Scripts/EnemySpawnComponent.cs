using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemySpawnComponent : MonoBehaviour
{
    [SerializeField] private List<EnemyWave> enemyWaves = new List<EnemyWave>();
    private EnemyWave currentEnemyWave;
    private int currentEnemyWaveIndex;
    private int aliveEnemies = 0;
    private int enemiesSpawned = 0;
    private float spawnEnemyTimer = 0;

    //For enemy spawn position
    private int maxSpawnAttempts = 10;
    [SerializeField] private float spawnRadius = 1.5f;
    private float checkRadius = 0.5f; //How big is the enemy for collision check
    [SerializeField] private LayerMask obstacleLayer;

    //Rune Activation
    [SerializeField] SpriteFlashing[] runes;

    private void Start()
    {
        //Animation
        if (enemyWaves.Count == 0) return;

        currentEnemyWave = enemyWaves[0];
        currentEnemyWaveIndex = 0;

        if(currentEnemyWaveIndex < runes.Length - 1)
        {
            runes[currentEnemyWaveIndex].EnableSpriteFlashing();
        }
    }

    private void Update()
    {
        if (!currentEnemyWave || !currentEnemyWave.enemyPrefab || enemyWaves.Count == 0) return;
        
        if(enemiesSpawned < currentEnemyWave.enemyCount)
        {
            spawnEnemyTimer -= Time.deltaTime;
            if (spawnEnemyTimer <= 0)
                SpawnEnemy();
        }
        else
        {
            if (aliveEnemies > 0) return;

            currentEnemyWave = null;
            if (currentEnemyWaveIndex < enemyWaves.Count - 1)
            {
                currentEnemyWaveIndex++;
                currentEnemyWave = enemyWaves[currentEnemyWaveIndex];
                enemiesSpawned = 0;
                spawnEnemyTimer = currentEnemyWave.restDelay;

                if (currentEnemyWaveIndex <= runes.Length - 1)
                {
                    runes[currentEnemyWaveIndex].EnableSpriteFlashing();
                }
            }
        }
    }

    private void SpawnEnemy()
    {
        
        if (!currentEnemyWave.enemyPrefab) return;

        Vector2 spawnPosition;
        bool foundValidPosition = TryGetValidSpawnPosition(out spawnPosition);
        if(!foundValidPosition)
        {
            spawnEnemyTimer = currentEnemyWave.spawnDelay;
            return;
        }

        GameObject enemy = Instantiate(currentEnemyWave.enemyPrefab, spawnPosition, Quaternion.identity);
        
        enemiesSpawned++;
        aliveEnemies++;
        if(enemy.TryGetComponent(out HealthComponent enemyHealth))
        {
            enemyHealth.OnEnemyDeath += EnemyKilled;
        }
        spawnEnemyTimer = currentEnemyWave.spawnDelay;
    }

    private bool TryGetValidSpawnPosition(out Vector2 position)
    {
        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            //Normalized picks a point ON the circle.
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            float randomDistance = Random.Range(0.5f, spawnRadius);
            Vector2 candidatePosition = (Vector2)transform.position + randomDirection * randomDistance;
            
            Debug.DrawLine(transform.position, candidatePosition, Color.yellow, 0.5f);

            // Check for walls or obstacles
            Collider2D hit = Physics2D.OverlapCircle(candidatePosition, checkRadius, obstacleLayer);
            if (hit == null)
            {
                position = candidatePosition;
                return true;
            }
        }

        position = Vector2.zero;
        return false;
    }

    private void EnemyKilled(GameObject killedEnemy)
    {
        if (killedEnemy.TryGetComponent(out HealthComponent enemyHealth))
        {
            enemyHealth.OnEnemyDeath -= EnemyKilled;
        }
        aliveEnemies--;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
