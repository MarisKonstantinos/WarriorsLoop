using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Waves/Wave")]
public class EnemyWave : ScriptableObject
{
    public int enemyCount;
    public float spawnDelay;
    [Tooltip("The delay between this wave and the next.")] 
    public float restDelay;
    public GameObject enemyPrefab;
}
