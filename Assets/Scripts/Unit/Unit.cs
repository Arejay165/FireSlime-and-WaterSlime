using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Unit : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] protected UnitStats unitStatTemplate;
    [Header("Stats")]
    [SerializeField] public bool isFacingRight;
   
    
    public int team;
    public int ID;
    public int interactionColor;

    public Health health;

    public bool isJumping;
    public bool canMove;
    [SerializeField] public float movementSpeed;
    [SerializeField] public float jumpForce;

    public LayerMask groundLayer;
    [Header("Components")]
    [SerializeField] public SpriteRenderer sr;
    [SerializeField] public Rigidbody2D rb;
    [SerializeField] public Transform groundChecker;
    public AudioSource audsrc;
    public bool hasKey;
    public virtual void InitializeStats()
    {
        if (unitStatTemplate)
        {
            team = unitStatTemplate.team;
            health = gameObject.GetComponent<Health>();
            if (health)
            {
                health.Initialize(unitStatTemplate.maxHealth);
            }
            
            canMove = true;
            movementSpeed = unitStatTemplate.movementSpeed;
        }
       
        sr = gameObject.GetComponent<SpriteRenderer>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        if (!groundChecker)
        {
            for (int i = 0; i < transform.childCount;i++)
            {
                if(transform.GetChild(i).gameObject.name == "GroundChecker")
                {
                    groundChecker = transform.GetChild(i).transform;
                }
            }
        }
        
    }


    public virtual void Flip(float directionX)
    {
        if (directionX < 0 && isFacingRight || directionX > 0 && !isFacingRight)
        {
            isFacingRight = !isFacingRight;
          
            gameObject.GetComponent<SpriteRenderer>().flipX = !gameObject.GetComponent<SpriteRenderer>().flipX;
       
            

        }
    }
    public bool isGrounded()
    {
        Collider2D gc = Physics2D.OverlapCircle(groundChecker.position, 0.5f, groundLayer);

        if (gc != null)
        {
            
          
            
            return true;
        }
        
        return false;

    }
    #region Unity Methods
    #endregion
}
