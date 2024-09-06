using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private float spawnRange = 9;
    public GameObject[] enemyPrefabs;
    public GameObject[] powerupPrefabs;
    public int enemyCount;
    public int waveCount;

    // Start is called before the first frame update
    void Start()
    {
        SpawnEnemyWave(waveCount);
        //int randomPowerUp = Random.Range(0, powerupPrefabs.Length);
        //Instantiate(powerupPrefabs[randomPowerUp], GenerateSpawnPos(), powerupPrefabs[randomPowerUp].transform.rotation);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        enemyCount = FindObjectsOfType<Enemy>().Length;

        if (enemyCount == 0)
        {
            waveCount++;
            SpawnEnemyWave(waveCount);
            int randomPowerUp = Random.Range(0, powerupPrefabs.Length);
            Instantiate(powerupPrefabs[randomPowerUp], GenerateSpawnPos(), powerupPrefabs[randomPowerUp].transform.rotation);
        }
    }

    private Vector3 GenerateSpawnPos()
    {
        float spawnPosX = Random.Range(-spawnRange, spawnRange);
        float spawnPosZ = Random.Range(-spawnRange, spawnRange);
        Vector3 randomPos = new Vector3(spawnPosX, 0, spawnPosZ);
        return randomPos;
    }

    void SpawnEnemyWave(int enemiesToSpawn)
    {
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            int r = Random.Range(0, 3);
            Debug.Log(r);
            if (r >= 1) { r = 1;}
            Instantiate(enemyPrefabs[r], GenerateSpawnPos(), Quaternion.identity);
        }
    }
}
