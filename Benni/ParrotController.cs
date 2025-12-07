using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParrotController : MonoBehaviour
{
    public float speed = 10f;      
    public float minY = -4.5f;      // Limit
    public float maxY = 4.5f;
    public float tiltAmount = 20f;  // Max tilt angle
    public float tiltSpeed = 5f;    // How fast tilt adjusts

    private float lastY;

    void Start()
    {
        lastY = transform.position.y;
    }

    void Update()
    {
        // Get cursor position
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Target position (X constant, Y follows cursor)
        Vector3 targetPos = new Vector3(transform.position.x, mousePos.y, transform.position.z);

        // Limit
        targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);

        // Move smoothly
        transform.position = Vector3.Lerp(transform.position, targetPos, speed * Time.deltaTime);

        // Tilt 
        float verticalMovement = transform.position.y - lastY;
        float targetTilt = Mathf.Clamp(verticalMovement * tiltAmount * 10f, -tiltAmount, tiltAmount);
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetTilt); // -1 to flip direction if needed
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, tiltSpeed * Time.deltaTime);
        lastY = transform.position.y;
    }
}
