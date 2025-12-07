using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raptor : MonoBehaviour
{
    public float speed = 5f;
    public float leftBound = -12f; 

    void Update()
    {       
        transform.Translate(Vector2.left * speed * Time.deltaTime);
        
        if (transform.position.x < leftBound)
        {
            Destroy(gameObject);
        }
    }
}
