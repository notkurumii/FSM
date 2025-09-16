using UnityEngine;

[System.Serializable]
public class EnemySpawnData
{
    [Header("Prefab Enemy")]
    public GameObject enemyPrefab;

    [Header("Probabilitas Spawn")]
    [Range(0f, 1f)]
    public float spawnProbability = 0.33f;
}

public class EnemySpawner : MonoBehaviour
{
    [Header("Tipe Musuh")]
    [Space]
    [Header("Merah")]
    public EnemySpawnData enemyMerah;
    [Header("Kuning")]
    public EnemySpawnData enemyKuning;
    [Header("Biru")]
    public EnemySpawnData enemyBiru;

    [Header("Pengaturan Spawn")]
    public int spawnCount = 5;
    public Vector3 spawnAreaCenter = Vector3.zero;
    public Vector3 spawnAreaSize = new Vector3(10, 0, 10);

    void Start()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        GameObject prefab = GetRandomEnemyPrefab();
        if (prefab == null) return;

        Vector3 randomPos = spawnAreaCenter + new Vector3(
            Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
            0,
            Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
        );

        Instantiate(prefab, randomPos, Quaternion.identity);
    }

    GameObject GetRandomEnemyPrefab()
    {
        EnemySpawnData[] allTypes = new EnemySpawnData[] { enemyMerah, enemyKuning, enemyBiru };
        float totalProb = 0f;
        foreach (var data in allTypes)
            totalProb += data.spawnProbability;

        float rand = Random.Range(0f, totalProb);
        float cumulative = 0f;
        foreach (var data in allTypes)
        {
            cumulative += data.spawnProbability;
            if (rand <= cumulative)
                return data.enemyPrefab;
        }
        return null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(spawnAreaCenter, spawnAreaSize);
    }
}