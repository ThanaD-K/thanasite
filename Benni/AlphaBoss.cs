using UnityEngine;

public class AlphaBoss : MonoBehaviour
{
    [Header("Movement")]
    public float speedY = 3f;          // Vertical movement speed
    public float minY = -4f;
    public float maxY = 4f;

    public float slideSpeed = 5f;      // Horizontal slide speed
    public float enterX = 10f;         // Start off-screen
    public float targetX = 7f;         // X position to stay while moving vertically
    public float exitX = 12f;          // X position to leave screen

    public float duration = 10f;       // Time to stay at targetX

    private bool verticalActive = false;
    private bool leaving = false;
    private bool movingUp = true;
    private float timer = 0f;

    public event System.Action OnBossEnd;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 7f;
    public float fireInterval = 2f;     // time between crescent waves
    private float fireTimer = 0f;

    [Header("Crescent Pattern")]
    public int crescentCount = 5;
    public float crescentAngleSpread = 60f;  // total spread angle of the crescent

    [Header("Circular Pattern")]
    public int circularCount = 12;          // number of bullets in the circle
    public float circularInterval = 4f;     // time between circular shots
    private float circularTimer = 0f;

    public GameObject playerObject; // drag prefab or scene object
    private Transform player;       // internal transform
    public AudioClip bossTheme;

    void Start()
    {
        AudioManager.Instance.PlayBossMusic(bossTheme);
        if (playerObject != null)
            player = playerObject.transform;
        // Spawn off-screen on right
        transform.position = new Vector3(enterX, transform.position.y, transform.position.z);
    }

    void Update()
    {
        if (!verticalActive)
        {
            // Slide in to targetX
            transform.Translate(Vector2.left * slideSpeed * Time.deltaTime, Space.World);
            if (transform.position.x <= targetX)
                verticalActive = true;
        }
        else if (verticalActive && !leaving)
        {
            // Vertical movement only
            if (movingUp)
            {
                transform.Translate(Vector2.up * speedY * Time.deltaTime, Space.World);
                if (transform.position.y >= maxY) movingUp = false;
            }
            else
            {
                transform.Translate(Vector2.down * speedY * Time.deltaTime, Space.World);
                if (transform.position.y <= minY) movingUp = true;
            }

            // Wait for duration
            timer += Time.deltaTime;
            if (timer >= duration)
                leaving = true;
        }
        else if (leaving)
        {
            // Slide back off-screen
            transform.Translate(Vector2.right * slideSpeed * Time.deltaTime, Space.World);
            if (transform.position.x >= exitX)
            {
                AudioManager.Instance.PlayStageMusic();
                OnBossEnd?.Invoke();
                Destroy(gameObject);
            }
        }

        // Shooting crescent pattern
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireInterval)
        {
            FireCrescent();
            fireTimer = 0f;
        }

        // Circular pattern shooting
        circularTimer += Time.deltaTime;
        if (circularTimer >= circularInterval)
        {
            FireCircular();
            circularTimer = 0f;
        }
    }
    void FireCrescent()
    {
        if (player == null) return;

        // Direction to player
        Vector2 dirToPlayer = (player.position - transform.position).normalized;
        float baseAngle = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg;

        // --- Crescent Spread ---
        float startAngle = baseAngle - crescentAngleSpread / 2f;
        float step = crescentAngleSpread / (crescentCount - 1);

        for (int i = 0; i < crescentCount; i++)
        {
            float angle = startAngle + step * i;
            Quaternion rot = Quaternion.Euler(0, 0, angle);
            GameObject bullet = Instantiate(bulletPrefab, transform.position, rot);

            // Move bullet along its rotation
            bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.right * bulletSpeed;
        }

        // --- Extra bullet directly toward player ---
        GameObject directBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        directBullet.GetComponent<Rigidbody2D>().velocity = dirToPlayer * bulletSpeed;
    }
    void FireCircular()
    {
        for (int i = 0; i < circularCount; i++)
        {
            float angle = i * (360f / circularCount);
            Quaternion rot = Quaternion.Euler(0, 0, angle);
            GameObject bullet = Instantiate(bulletPrefab, transform.position, rot);

            // Move bullet along its rotation
            bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.right * bulletSpeed;
        }
    }
}
