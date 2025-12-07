using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSkillManager : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 5f;
    public float attackInterval = 1f; // Time between attacks

    public float moveSpeed = 2f;
    public float moveInterval = 3f;
    public float minX, maxX, minY, maxY;

    private bool isAttacking = false;
    private bool isMoving = false;
    private Vector2 targetPosition;
    private Coroutine attackCoroutine;
    private Coroutine moveCoroutine;
    private Vector2 originalPosition;

    // Bullet count variables (editable in Inspector)
    public int singleShotBullets = 1;  // Used for Phase 1 
    public int spreadShotBullets = 5;  // Used for Phase 2 
    public int circularShotBullets = 12; // Used for Phase 3 

    void Start()
    {
        originalPosition = transform.position; // Store the original position at the start
    }

    public void ActivateSkill(int phase)
    {
        StopAttacking(); // Stop attacks and movement before switching phase
        StartCoroutine(ReturnToOriginalPosition(() => StartPhase(phase))); // Return to position before starting next phase
    }

    private void StartPhase(int phase)
    {
        isAttacking = true;
        switch (phase)
        {
            case 1:
                StartCoroutine(DelayedStart());
                StartMovement();
                break;
            case 2:
                attackCoroutine = StartCoroutine(ShootMultipleBullets(spreadShotBullets));
                StartMovement();
                break;
            case 3:
                attackCoroutine = StartCoroutine(ShootCircularPattern(circularShotBullets));
                StartMovement();
                break;
        }
    }
    IEnumerator DelayedStart()
    {

        yield return new WaitForSeconds(1f);
        attackCoroutine = StartCoroutine(ShootSingleBullet(singleShotBullets));

    }
    private IEnumerator ShootSingleBullet(int bulletCount)
    {
        while (isAttacking)
        {
            for (int i = 0; i < bulletCount; i++)
            {
                ShootAtPlayer();
            }
            yield return new WaitForSeconds(attackInterval);
        }
    }

    private IEnumerator ShootMultipleBullets(int bulletCount)
    {
        while (isAttacking)
        {
            ShootSpreadAtPlayer(bulletCount, 20f); // Spread adjustable
            yield return new WaitForSeconds(attackInterval);
        }
    }

    private IEnumerator ShootCircularPattern(int bulletCount)
    {
        while (isAttacking)
        {
            ShootInCircle(bulletCount);
            yield return new WaitForSeconds(attackInterval);
        }
    }

    private void ShootAtPlayer()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;

        Vector2 direction = (player.transform.position - firePoint.position).normalized;
        FireBullet(direction);
    }

    private void ShootSpreadAtPlayer(int bulletCount, float angleSpread)
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;

        Vector2 direction = (player.transform.position - firePoint.position).normalized;
        float halfSpread = angleSpread / 2f;

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = Mathf.Lerp(-halfSpread, halfSpread, (float)i / (bulletCount - 1));
            Vector2 spreadDirection = Quaternion.Euler(0, 0, angle) * direction;
            FireBullet(spreadDirection);
        }
    }

    private void ShootInCircle(int bulletCount)
    {
        for (int i = 0; i < bulletCount; i++)
        {
            float angle = (360f / bulletCount) * i;
            Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.up;
            FireBullet(direction);
        }
    }

    private void FireBullet(Vector2 direction)
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = direction * bulletSpeed;
    }

    private void StartMovement()
    {
        isMoving = true;
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveRandomly());
    }

    private IEnumerator MoveRandomly()
    {
        while (isMoving)
        {
            SetRandomTargetPosition();
            while (Vector2.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }
            yield return new WaitForSeconds(moveInterval);
        }
    }

    private void SetRandomTargetPosition()
    {
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);
        targetPosition = new Vector2(randomX, randomY);
    }

    public void StopAttacking()
    {
        isAttacking = false;
        isMoving = false;

        if (attackCoroutine != null) StopCoroutine(attackCoroutine);
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
    }

    // Move the enemy back to the original position before the next phase starts
    private IEnumerator ReturnToOriginalPosition(System.Action onComplete)
    {
        while (Vector2.Distance(transform.position, originalPosition) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, originalPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = originalPosition;
        onComplete?.Invoke(); // Call next phase after returning
    }
}
