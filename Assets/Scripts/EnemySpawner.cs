using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class EnemySpawner : MonoBehaviour
{
    // Start is called before the first frame update
    public Tilemap tilemap;
    public float spawnRadius = 5.0f;
    public GameObject enemyPrefab;
    public float spawnInterval = 0.2f;

    public IntVariable waveCounter;
    public UnityEvent waveUpdate;

    private Vector3 _startPosition;
    private bool _destroyed = false;
    private bool _spawning = false;
    
    private int _lastSpawnedWave = 0;
    private int _enemiesKilled = 0;
    private int _spawned = 0;

    void Start()
    {
        waveCounter.SetValue(0);
        _startPosition = transform.position;
        StartCoroutine(SpawnAttackWave());
    }

    public void OnEnemyKilled() {
        _enemiesKilled += 1;
        
        /*
        if ((_enemiesKilled == _spawned) && !_spawning) {
            Debug.Log("WAVE IS OVER");
            StartCoroutine(SpawnAttackWave());
            waveUpdate.Invoke();
        }
        else
        {
            Debug.Log("WAVE KILLED " + _enemiesKilled);
            Debug.Log("WAVE SPAWNED " + _spawned);
        }*/

        bool hasLivingEnemies = false;
        
        foreach (Transform child in transform)
        {
            bool alive = child.GetComponent<EnemyController>().IsAlive();
            if (alive)
            {
                hasLivingEnemies = true;
                break;
            }
        }
        
        if (!hasLivingEnemies && !_spawning) {
            Debug.Log("WAVE IS OVER");
            StartCoroutine(SpawnAttackWave());
            waveUpdate.Invoke();
        }
    }

    IEnumerator SpawnAttackWave()
    {
        /*
         * spawn slimes in an attack wave
         */
        while (_spawning) {
            // wait for previous wave to finish spawning
            yield return null;
        }
        
        _spawning = true;
        _enemiesKilled = 0;
        waveCounter.Increment();
        _spawned = 0;
        
        while (_spawned < waveCounter.Value)
        {
            float offsetX = 2 * Random.Range(0.0f, spawnRadius) - spawnRadius;
            float offsetY = 2 * Random.Range(0.0f, spawnRadius) - spawnRadius;
            Vector3 spawnPosition = new Vector3(
                _startPosition.x + offsetX,
                _startPosition.y + offsetY,
                _startPosition.z
            );

            bool spawnable = InTilemap(spawnPosition);
            if (spawnable) {
                GameObject enemy = Instantiate(
                    enemyPrefab, spawnPosition,
                    Quaternion.identity
                );

                enemy.transform.SetParent(transform, true);
                _spawned += 1;
            }

            yield return new WaitForSeconds(spawnInterval);
        }

        _spawning = false;
        yield return null;
    }
    
    private bool InTilemap(Vector3 position) {
        // check if the spawn position is in spawnable tile area
        Vector3Int cellPosition = tilemap.WorldToCell(position);
        return tilemap.HasTile(cellPosition);
    }

    private void OnDestroy()
    {
        _destroyed = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
