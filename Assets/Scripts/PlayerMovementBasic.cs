using System;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovementTopDown : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;
    public Vector2 lastMoveDir { get; private set; }
    public PlayerCarry PlayerCarry;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Time.fixedDeltaTime = 0.001f;
    }

    void Update() // esto sucede cada frame 
    {
        Debug.Log(PlayerCarry.isCollision);

        // Movimiento en los ejes X y Y
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        if (movement.sqrMagnitude > 0.01f)
        {
            Debug.Log($"Movimiento y: " + movement.y + "Movimiento x: " + movement.x + "Movimiento Magnitud: " + movement.magnitude);
            lastMoveDir = movement;           
        }
    }

    void FixedUpdate() // se ejecuta a intervalor fijos
    {
        
        if(PlayerCarry.isCollision == false)
        {
            if(PlayerCarry.isCarrying != false)
            {
                // Movimiento suave sin gravedad
                speed = speed; 
                rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
            }
            else
            {
                rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
            }
            
        }
       
    }
}
