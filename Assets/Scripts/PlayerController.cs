using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class PlayerController : MonoBehaviourPun
{
    [SerializeField] private Player _playerRef;
    [SerializeField] private Vector2 _movementDirection;
    [SerializeField] private float jumpForce;

    public void Start()
    {

      
            _playerRef = this.transform.gameObject.GetComponent<Player>();
            _movementDirection = new Vector3(0f, 0f, 0f);
        
    }
    public void OnEnable()
    {

    }

    public void OnDisable()
    {

    }
    // Update is called once per frame
    void Update()
    {
        //Inputs
        if (GameManager.instance.isPlaying == true) //If game is playing and not paused
        {
            if (_playerRef) //If playerRef is not null
            {
                if (photonView.IsMine)
                {

                    if (_playerRef.isGrounded() || _playerRef.isColoredGrounded())
                    {
                        if (Input.GetKeyDown(KeyCode.Space))
                        {
                            if (_playerRef.canJump)
                            {
                                Jump();
                            }    
                            
                           
                        } 
                                
                            
                    
                    }
             
                }
                


            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {

            GameManager.instance.plrUI.OpenUi();

        }

        //   Debug.Log(_playerRef.isGrounded());
        // Debug.Log( "Color"+ _playerRef.isColoredGrounded());
    }

    private void FixedUpdate()
    {

        if (GameManager.instance.isPlaying == true) //If game is playing and not paused
        {
            if (_playerRef) //If playerRef is not null
            {
                if (photonView.IsMine)
                {
                    //Walking
                    if (_playerRef.canMove)
                    {
                        _movementDirection = new Vector2(Input.GetAxisRaw("Horizontal"), 0f);
                        _movementDirection.Normalize();

                        _playerRef.Flip(_movementDirection.x);

                        if (_playerRef.rb)
                        {
                            _playerRef.rb.velocity = new Vector2(_movementDirection.x * _playerRef.movementSpeed, _playerRef.rb.velocity.y);
                        }
                        

                    }
                    if (_playerRef.attachedToGround)
                    {
                        if (_playerRef.rb)
                        {
                            _playerRef.rb.velocity = _playerRef.rb.velocity + _playerRef.attachedToGround.velocity;
                        }
                    }
                    

                }
            }
        }

    }

    public void Jump()
    {
        AudioManager.instance.Play("SlimeJump"); //[AUDIO TO FILL UP]
        _playerRef.anim.SetTrigger("IsJump");
        _playerRef.rb.velocity = Vector2.up * jumpForce;
        //Collider2D[] colliders = Physics2D.OverlapCircleAll(_playerRef.groundChecker.position, 1f, _playerRef.groundLayer);
        //if (colliders.Length <= 0)
        //{
           
        //    transform.parent = null;
        //    _playerRef.attachedToGround = null;
            
        //    //_playerRef.rb.velocity = 

        //}
    }
  
}
