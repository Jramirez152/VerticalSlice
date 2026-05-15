using UnityEngine;
using Unity.VisualScripting;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject enemyPrefab;
    public GameObject enemyFastPrefab;
    public GameObject enemyBigPrefab;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Wave Settings")]
    public float spawnInterval = 2f;

    [Header("Graph Target")]
    public GameObject graphTarget; 

    public int currentWave { get; private set; } = 0;
    public bool allWavesComplete { get; private set; } = false;

    private int[][] _waveComposition = new int[][]
    {
        new int[] { 3, 0, 0 },
        new int[] { 3, 2, 0 },
        new int[] { 2, 2, 1 }
    };

    private float _spawnTimer;
    private int _spawnIndex;
    private int _enemiesSpawnedThisWave;
    private GameObject[] _spawnQueue;
    private int _lastEnemyCount = 0;

    void Start()
    {
        StartWave(0);
    }

    void Update()
    {
        if (allWavesComplete) return;

        int currentEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;

        if (currentEnemies != _lastEnemyCount)
        {
            _lastEnemyCount = currentEnemies;
            FireUpdateEvent(currentWave + 1, currentEnemies);
        }

        if (_enemiesSpawnedThisWave < _spawnQueue.Length)
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
            if (nextWave >= _waveComposition.Length)
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
        _spawnIndex = 0;
        _spawnQueue = BuildSpawnQueue(waveIndex);
        FireUpdateEvent(currentWave + 1, 0);
    }

    void FireUpdateEvent(int wave, int enemyCount)
    {
        if (graphTarget != null)
        {
            EventBus.Trigger(
                new EventHook("OnWaveUpdate", graphTarget),
                new WaveUpdateArgs(wave, enemyCount)
            );
        }
    }

    GameObject[] BuildSpawnQueue(int waveIndex)
    {
        int[] composition = _waveComposition[waveIndex];
        int total = composition[0] + composition[1] + composition[2];
        GameObject[] queue = new GameObject[total];

        int index = 0;
        for (int i = 0; i < composition[0]; i++) queue[index++] = enemyPrefab;
        for (int i = 0; i < composition[1]; i++) queue[index++] = enemyFastPrefab;
        for (int i = 0; i < composition[2]; i++) queue[index++] = enemyBigPrefab;

        for (int i = queue.Length - 1; i > 0; i--)
        {
            int rand = Random.Range(0, i + 1);
            GameObject temp = queue[rand];
            queue[rand] = queue[i];
            queue[i] = temp;
        }

        return queue;
    }

    void SpawnEnemy()
    {
        if (spawnPoints.Length == 0 || _spawnQueue == null) return;

        GameObject prefabToSpawn = _spawnQueue[_enemiesSpawnedThisWave];
        if (prefabToSpawn == null) return;

        Transform point = spawnPoints[_spawnIndex % spawnPoints.Length];
        Instantiate(prefabToSpawn, point.position, Quaternion.identity);

        _spawnIndex++;
        _enemiesSpawnedThisWave++;
    }
}