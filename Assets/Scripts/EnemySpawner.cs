
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Setup")]
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;

    [Header("Wave Settings")]
    public int[] enemiesPerWave = { 3, 5, 7 }; 
    public float spawnInterval = 2f;

    public int currentWave { get; private set; } = 0;
    public bool allWavesComplete { get; private set; } = false;

    private float _spawnTimer;
    private int _spawnIndex;
    private int _enemiesSpawnedThisWave;

    void Start()
    {
        StartWave(0);
    }

    void Update()
    {
        if (allWavesComplete) return;

        int currentEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        int enemiesToSpawnThisWave = enemiesPerWave[currentWave];

        if (_enemiesSpawnedThisWave < enemiesToSpawnThisWave)
        {
            _spawnTimer -= Time.deltaTime;
            if (_spawnTimer <= 0f)
            {
                SpawnEnemy();
                _spawnTimer = spawnInterval;
            }
        }
        else if (currentEnemies == 0)
        {
            int nextWave = currentWave + 1;
            if (nextWave >= enemiesPerWave.Length)
            {
                allWavesComplete = true;
                GameManager.Instance?.OnAllWavesComplete();
            }
            else
            {
                StartWave(nextWave);
            }
        }
    }

    void StartWave(int waveIndex)
    {
        currentWave = waveIndex;
        _enemiesSpawnedThisWave = 0;
        _spawnTimer = 1f; 
    }

    void SpawnEnemy()
    {
        if (spawnPoints.Length == 0 || enemyPrefab == null) return;

        Transform point = spawnPoints[_spawnIndex % spawnPoints.Length];
        Instantiate(enemyPrefab, point.position, Quaternion.identity);
        _spawnIndex++;
        _enemiesSpawnedThisWave++;
    }
}