using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
    
{
    public float movespeed;
    Rigidbody2D rb;
    Vector2 Dir;
    bool flip = true;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
        InputManage();

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if(mousePos.x>transform.position.x && flip)
        {
            Flips();
        }
        else if(mousePos.x<transform.position.x && !flip)
        {
            Flips();
        }
    }
    void FixedUpdate()
    {
        Move();
    }

    void InputManage()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Dir = new Vector2(moveX, moveY).normalized;
    }

    void Move()
    {
        rb.velocity = new Vector2(Dir.x * movespeed, Dir.y * movespeed);
    }
    void Flips()
    {
        flip = !flip;
        transform.Rotate(0f, 180f, 0f);
    }
}
