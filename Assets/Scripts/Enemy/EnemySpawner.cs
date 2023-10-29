using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public List<EnemyGroup> enemyGroups; //list of groups enemies to spawn in this wave
        public int waveQuota; //total number of enemies in wave
        public float spawnInterval; //the interval at which to spawn enemies
        public int spawnCount; //number of enemies already spawned
    }

    [System.Serializable]
    public class EnemyGroup
    {
        public string enemyName;
        public int enemyCount; //number of enemies to spawn in wave
        public int spawnCount; //number of enemies of this type already spawned
        public GameObject enemyPrefabs; //enemy prefabs for wave
    }

    public List<Wave> waves; //list of all waves in game
    public int currentWaveCount; //index of the current wave, start from 0

    [Header("Spawner Attributes")]
    float spawnTimer; //time use to determine when to next spawn
    public int enemiesAlive;
    public int maxEnemiesAllowed; //maximum number of enemies allowed on the map at once
    public bool maxEnemiesReached = false; //flag for determine if maximum number of enemies has beend reached
    public float waveInterval; //interval between each wave

    bool isWaveActive = false;

    [Header("Spawn Positions")]
    public List<Transform> relativeSpawnPoints; //list to store all the spawn points of enemies

    Transform player;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerStats>().transform;
        CalculateWaveQuota();
    }

    // Update is called once per frame
    void Update()
    {
        //check if the wave has ended and the next wave should start
        if(currentWaveCount < waves.Count && waves[currentWaveCount].spawnCount == 0 && !isWaveActive)
        {
            StartCoroutine(BeginNextWave());
        }

        spawnTimer += Time.deltaTime;
        //check if its time to spawn next enemy
        if(spawnTimer >= waves[currentWaveCount].spawnInterval)
        {
            spawnTimer = 0f;
            SpawnEnemies();
        }
    }

    IEnumerator BeginNextWave()
    {
        isWaveActive = true;

        //wait "waveInternal" seconds before starting next wave
        yield return new WaitForSeconds(waveInterval);
        //if there are more waves to start after the current wave, then move on to the next wave
        if(currentWaveCount < waves.Count - 1)
        {
            isWaveActive = false;
            currentWaveCount++;
            CalculateWaveQuota();
        }
    }

    void CalculateWaveQuota()
    {
        int currentWaveQuota = 0;
        foreach(var enemyGroup in waves[currentWaveCount].enemyGroups)
        {
            currentWaveQuota += enemyGroup.enemyCount;
        }

        waves[currentWaveCount].waveQuota = currentWaveQuota;
        Debug.LogWarning(currentWaveQuota);
    }

    /// <summary>
    /// This method will stop spawning enemies if the amount of enemies on the map is maximum.
    /// The method will only spawn enemies in a particular wave untril it is time for the next wave's enemies to be spawned.
    /// </summary>
    void SpawnEnemies()
    {
        //check if the minimum number of enemies in the wave have been spawned
        if (waves[currentWaveCount].spawnCount < waves[currentWaveCount].waveQuota && !maxEnemiesReached)
        {
            //spawn each of type enemy until the quota is filled
            foreach (var enemyGroup in waves[currentWaveCount].enemyGroups)
            {
                //check if the minimum number of enemies of this type have been spawned
                if(enemyGroup.spawnCount < enemyGroup.enemyCount)
                {
                    //spawn the enemies at a random position close to player
                    Instantiate(enemyGroup.enemyPrefabs, player.position + relativeSpawnPoints[Random.Range(0, relativeSpawnPoints.Count)].position, Quaternion.identity);              

                    enemyGroup.spawnCount++;
                    waves[currentWaveCount].spawnCount++;
                    enemiesAlive++;

                    //limit the number of enemies can be spawned at once
                    if (enemiesAlive >= maxEnemiesAllowed)
                    {
                        maxEnemiesReached = true;
                        return;
                    }
                }
            }
        }
    }

    //call when enemies dead
    public void OnEnemyKilled()
    {
        //decrement the number of enemies alive
        enemiesAlive--;

        //reset the flag if the number of enemies alive is below the maximum amount
        if (enemiesAlive < maxEnemiesAllowed)
        {
            maxEnemiesReached = false;
        }
    }
}
