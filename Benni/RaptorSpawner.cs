using UnityEngine;

public class RaptorSpawner : MonoBehaviour
{
    [Header("Raptors")]
    public GameObject raptorPrefab;
    public float minY = -4.5f;
    public float maxY = 4.5f;
    public float spawnInterval = 2f;

    [Header("Bosses")]
    public GameObject[] bossPrefabs;   // <<< multiple boss prefabs
    public float bossInterval = 15f;   // Time before boss appears

    private float spawnTimer = 0f;
    private float bossTimer = 0f;
    private bool bossActive = false;

    void Start()
    {
        AudioManager.Instance.PlayStageMusic();
    }
    void Update()
    {
        spawnTimer += Time.deltaTime;
        bossTimer += Time.deltaTime;

        if (!bossActive && bossTimer >= bossInterval)
        {
            SpawnBoss();
            bossTimer = 0f;   // reset boss timer after spawning
        }

        if (!bossActive && spawnTimer >= spawnInterval)
        {
            SpawnRaptor();
            spawnTimer = 0f;  // reset spawn timer after spawning
        }
    }

    void SpawnRaptor()
    {
        float randomY = Random.Range(minY, maxY);
        Vector3 spawnPos = new Vector3(transform.position.x, randomY, 0);
        Instantiate(raptorPrefab, spawnPos, Quaternion.identity);
    }

    void SpawnBoss()
    {
        bossActive = true;

        // pick random boss from array
        int randIndex = Random.Range(0, bossPrefabs.Length);
        GameObject bossPrefab = bossPrefabs[randIndex];

        Vector3 spawnPos = new Vector3(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x + 2f, 0, 0);
        GameObject boss = Instantiate(bossPrefab, spawnPos, Quaternion.identity);

        // Subscribe to boss end event (if it has AlphaBoss script)
        AlphaBoss bossScript = boss.GetComponent<AlphaBoss>();
        if (bossScript != null)
            bossScript.OnBossEnd += BossEnded;

        AlphaBoss2 bossScript2 = boss.GetComponent<AlphaBoss2>();
        if (bossScript2 != null)
            bossScript2.OnBossEnd += BossEnded;
        AlphaBoss3 bossScript3 = boss.GetComponent<AlphaBoss3>();
        if (bossScript3 != null)
            bossScript3.OnBossEnd += BossEnded;
    }

    void BossEnded()
    {
        bossActive = false;

        // Reset timers so raptors start spawning immediately
        spawnTimer = 0f;
        bossTimer = 0f;
    }
}
