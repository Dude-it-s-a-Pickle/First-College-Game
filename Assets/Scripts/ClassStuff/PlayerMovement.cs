using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public Rigidbody2D rb;
    [SerializeField] private float horMove = 0f;
    public float runSpeed;
    public float jumpMult;
    public Transform groundCheck;
    private bool onGround;
    public LayerMask ground;


    void Start()
    {
        
        rb = GetComponent<Rigidbody2D>();

    }
    
    void Update()
    {

        onGround = Physics2D.BoxCast(new Vector2(groundCheck.position.x, groundCheck.position.y), new Vector2(0.9f, 0.3f), 0f, Vector2.down, 0.5f, ground);

        Debug.Log(onGround);

        horMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        if (Input.GetKey(KeyCode.Space) && onGround)
        {

            Debug.Log(onGround);

            rb.AddForce(new Vector2(0, jumpMult));

        }

        

    }

    private void FixedUpdate()
    {

        rb.velocity = new Vector2(horMove * runSpeed, rb.velocity.y);

    }
}
