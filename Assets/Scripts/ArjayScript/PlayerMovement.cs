using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update

    Rigidbody2D rb2d;
    [SerializeField]float moveHorizontal;
    [SerializeField]float moveVertical;
    [SerializeField]float speed = 10f;
    [SerializeField] float jumpForce;
    public Color playerColor;
    public int collectedCoins;
    public bool hasKey;
    private bool canJump;
    public LayerMask groundLayer;
    public Transform coloredFeet;
    void Start()
    {
        rb2d = gameObject.GetComponent<Rigidbody2D>();
        hasKey = false;
    }

    // Update is called once per frame

    private void Update()
    {
        if (Input.GetButtonDown("Jump") && isGrounded())
        {
            Jump();
        }


    }
    private void FixedUpdate()
    {
        moveHorizontal = Input.GetAxis("Horizontal");
        moveVertical = Input.GetAxis("Vertical");
        Vector2 movement = new Vector2(moveHorizontal * speed, rb2d.velocity.y);

        rb2d.position += movement * Time.fixedDeltaTime;
    }

    public void Jump()
    {
   
        rb2d.velocity = Vector2.up * jumpForce;

    }

    public void AddCoins()
    {
        collectedCoins += 1;
    }

    public bool isGrounded()
    {
        Collider2D groundChecker = Physics2D.OverlapCircle(coloredFeet.position, 0.5f, groundLayer);

        if (groundChecker != null)
        {

            return true;
        }

        return false;

    }
}
