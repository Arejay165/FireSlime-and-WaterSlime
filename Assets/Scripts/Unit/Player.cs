using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Player : MonoBehaviourPunCallbacks, IPunObservable
{

  
    [Header("Stats")]
    [SerializeField] public bool isFacingRight;

    public int coins;


    public Health health;

    public bool canJump;
    public bool canMove;
    public bool hasKey;
    public int playerColorID;
    public int layerColorID;
    [SerializeField] public float movementSpeed;
    [SerializeField] public float jumpForce;

    public LayerMask groundLayer;
    public LayerMask coloredLayer;
    [Header("Components")]
   
    [SerializeField] public SpriteRenderer sr;
    [SerializeField] public Rigidbody2D rb;
    [SerializeField] public BoxCollider2D bc;
    [SerializeField] public Transform groundChecker;
    [SerializeField] public Transform coloredChecker;

    public Rigidbody2D attachedToGround;
    public AudioSource audsrc;
    public Animator anim;
    public int playerIndex;
    public override void OnConnectedToMaster()
    {
       
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    private void Start()
    {
       
    }
    public override void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
        InitializeStats();

        GetComponent<PlayerController>().enabled = photonView.IsMine;
        if (GetComponent<PhotonRigidbody2DView>())
        {
            GetComponent<PhotonRigidbody2DView>().enabled = photonView.IsMine;

        }
 

    }

    public override void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Interactible"))
        {
           
            if (collision.gameObject.GetComponent<MovingPlatform>())
            {
                
                Collider2D[] colliders = Physics2D.OverlapCircleAll(groundChecker.position, 0.35f, groundLayer);
                if (colliders.Length > 0)
                {
                    foreach (var c in colliders)
                    {
                        if (c.gameObject.GetComponent<MovingPlatform>())
                        {

                            attachedToGround = c.gameObject.GetComponent<Rigidbody2D>();

                        }

                    }

                }
                collision.gameObject.GetComponent<Interactible>().colorIndex = playerIndex;
                collision.gameObject.GetComponent<Interactible>().DoInteraction(photonView.ViewID);
            }
            else if (collision.gameObject.GetComponent<LeverControlled>())
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(groundChecker.position, 0.35f, groundLayer);
                if (colliders.Length > 0)
                {
                    foreach (var c in colliders)
                    {
                        if (c.gameObject.GetComponent<LeverControlled>())
                        {

                            if (c.gameObject.GetComponent<LeverControlled>().type == LeverControlledInteractibleType.platform)
                            {

                                attachedToGround = c.gameObject.GetComponent<Rigidbody2D>();
                            }



                        }
                    }

                }
                

            }

            else if (collision.gameObject.GetComponent<Interactible>())
            {


                collision.gameObject.GetComponent<Interactible>().colorIndex = playerIndex;
             


                

                if (collision.gameObject.GetComponent<Platform>() && collision.gameObject.GetComponent<Platform>().reactionType == ReactionType.staticColorChange)
                {
                    collision.gameObject.GetComponent<Interactible>().DoInteraction(playerIndex);

                    return;
                }
                else
                {
                    collision.gameObject.GetComponent<Interactible>().DoInteraction(photonView.ViewID);
                }


                    //  collision.gameObject.GetComponent<PhotonView>().RPC("DoInteraction", RpcTarget.AllBuffered, photonView.ViewID)
                    // Debug.Log("Iscollide");
                




            }
            
            
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Interactible"))
        {

            if (collision.gameObject.GetComponent<MovingPlatform>() || collision.gameObject.GetComponent<LeverControlled>())
            {
                
                attachedToGround = null;
            }
        }
    }

    private void OnTriggerEnter2D (Collider2D collision)
    {
       
        if (collision.gameObject.CompareTag("Interactible"))
        {
          
            if (collision.gameObject.GetComponent<Interactible>())
            {

                
        

                collision.gameObject.GetComponent<Interactible>().colorIndex = playerIndex;

                //collision.gameObject.GetComponent<Interactible>().DoInteraction(photonView.ViewID);
                if (photonView.IsMine)
                {


                    collision.gameObject.GetComponent<PhotonView>().RPC("DoFunction", RpcTarget.AllBuffered, photonView.ViewID);

                    if (collision.gameObject.GetComponent<Door>())
                    {
                        //collision.gameObject.GetComponent<PhotonView>().RPC("DoFunction", RpcTarget.AllBuffered, photonView.ViewID);
                        //   collision.gameObject.GetComponent<PhotonView>().RPC("DoFunction", RpcTarget.AllBuffered, playerIndex);
                        collision.gameObject.GetComponent<Door>().DoInteraction(photonView.ViewID);
                        return;
                    }
                  

                }
      

            }

        }
        
   
    }


    public virtual void InitializeStats()
    {
        health = gameObject.GetComponent<Health>();
        if (health)
        {
            health.Initialize(1);
        }
        
        ExitGames.Client.Photon.Hashtable newProperties = new ExitGames.Client.Photon.Hashtable()
                {
                    {
                        Constants.PLAYER_LEVEL, GameManager.instance.currentLevel

                    }
                };

        PhotonNetwork.LocalPlayer.SetCustomProperties(newProperties);
        sr = gameObject.GetComponent<SpriteRenderer>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        bc = gameObject.GetComponent<BoxCollider2D>();




        GameManager.instance.playerFounded.Add(this.gameObject);
        anim = gameObject.GetComponent<Animator>();
 
        canMove = true;
        canJump = true;
        hasKey = false;
        movementSpeed = 10;
        photonView.RPC("GetPlayerColorIndex", RpcTarget.AllBuffered);
        if (!groundChecker)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).gameObject.name == "GroundChecker")
                {
                    groundChecker = transform.GetChild(i).transform;
                }
            }
        }
        if (GameManager.instance.hasCutscene == true)
        {
            GameManager.instance.hidePlayer();
            
        }

    }


    [PunRPC]
    public void FinishedLevel()
    {
        canMove = false;
        sr.enabled = false;
        object playerProperty;
        if (photonView.Owner.CustomProperties.TryGetValue(Constants.PLAYER_SELETION_NUMBER, out playerProperty))
        {
        
            if ((int)playerProperty == 0)
            {
                GameManager.instance.blueFinished = true;
            }
            else if ((int)playerProperty == 1)
            {
                GameManager.instance.redFinished = true;
          
            }
            if (GameManager.instance.redFinished == true && GameManager.instance.blueFinished == true)
            {

                object[] dataPlacement = new object[] { };
                RaiseEventOptions raiseEventsOptionsPlacement = new RaiseEventOptions
                {
                    Receivers = ReceiverGroup.All,
                    CachingOption = EventCaching.AddToRoomCache
                };

                SendOptions sendOptionPlacement = new SendOptions { Reliability = true };

                PhotonNetwork.RaiseEvent((byte)Constants.RaiseEventsCode.GameWon, dataPlacement, raiseEventsOptionsPlacement, sendOptionPlacement);


                //PhotonNetwork.LoadLevel("Empty"); /// Need this empty scene to properly https://forum.photonengine.com/discussion/10484/reloading-scene-issue
                if (PhotonNetwork.IsMasterClient)
                {
                    StartCoroutine(delay());
                    
                }

               



            }
        }

    }

    
    public void Restart()
    {
        
        StartCoroutine(restartDelay());
        
    }
    IEnumerator restartDelay()
    {
        yield return new WaitForSeconds(5);


        //PhotonNetwork.LoadLevel("Empty");
        SceneManager.LoadSceneAsync(GameManager.instance.currentLevel);
        //PhotonNetwork.(GameManager.instance.currentLevel);
        

    }

    IEnumerator delay()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene(GameManager.instance.nextLevel);
        //PhotonNetwork.LoadLevel(GameManager.instance.nextLevel);
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            if (sr)
            {
                stream.SendNext(sr.flipX);
                stream.SendNext(transform.position);
            }
            
        }
        else
        {

            if (sr)
            {
                sr.flipX = (bool)stream.ReceiveNext();
                transform.position = (Vector3)stream.ReceiveNext();
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
        if (groundChecker)
        {
            Collider2D gc = Physics2D.OverlapCircle(groundChecker.position, 0.35f, groundLayer);

            if (gc != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }    
        
    }

    public bool isColoredGrounded()
    {
        Collider2D[] gc = Physics2D.OverlapCircleAll(coloredChecker.position, 0.35f, coloredLayer);
        if (gc.Length > 1)
        {
            foreach (Collider2D gcDetected in gc)
            {
                if (gcDetected.gameObject != gameObject)
                {
                    return true;
                }

                //if (gc != null)
                //{
                //    return true;
                //}
                //else
                //{
                //    return false;
                //}
            }
            return false;
        }
        else
        {
            return false;
        }


    }

    private void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == (byte)Constants.RaiseEventsCode.GameWon)
        {

            object[] data = (object[])photonEvent.CustomData;
            GameManager.instance.isPlaying = false;
            Text winnerText = GameManager.instance.winnerText;
            winnerText.gameObject.SetActive(true);

            winnerText.text = " GAME WON ";
            winnerText.color = Color.red;
            if (photonView.IsMine)
            {
                AudioManager.instance.Play("Win"); //[AUDIO TO FILL UP] 
            }
            //Text blueCoinText = GameManager.instance.redCoinText;
            //blueCoinText.gameObject.SetActive(true);
            //blueCoinText.text = GameManager.instance.blueCoin + " Coins";

            //Text redCoinText = GameManager.instance.blueCoinText;
            //redCoinText.gameObject.SetActive(true);
            //redCoinText.text = GameManager.instance.redCoin + " Coins";



        }
        else if (photonEvent.Code == (byte)Constants.RaiseEventsCode.GameLost)
        {
            GameManager.instance.isPlaying = false;
            object[] data = (object[])photonEvent.CustomData;
            Text winnerText = GameManager.instance.winnerText;
            winnerText.gameObject.SetActive(true);

            winnerText.text = " GAME LOST ";
            winnerText.color = Color.red;

            //Text blueCoinText = GameManager.instance.redCoinText;
            //blueCoinText.gameObject.SetActive(true);
            //blueCoinText.text = GameManager.instance.blueCoin + " Coins";

            //Text redCoinText = GameManager.instance.blueCoinText;
            //redCoinText.gameObject.SetActive(true);
            //redCoinText.text = GameManager.instance.redCoin + " Coins";
;
            
        }
    }
    
    [PunRPC]
    public void AddCoins()
    {
        AudioManager.instance.Play("Coin"); //[AUDIO TO FILL UP] //
        object playerProperty;
        if (photonView.Owner.CustomProperties.TryGetValue(Constants.PLAYER_SELETION_NUMBER, out playerProperty))
        {
            if ((int)playerProperty == 0)
            {
                GameManager.instance.blueCoin +=1;
                if (GameManager.instance.blueCoin >= GameManager.instance.maxBlueCoins)
                {
                    if (hasKey == false)
                    {
                        hasKey = true;
                       
                        
                        
                        
                    }
                }
            }
            else if ((int)playerProperty == 1)
            {
                GameManager.instance.redCoin +=1;
                if (GameManager.instance.redCoin >= GameManager.instance.maxRedCoins)
                {
                    if (hasKey == false)
                    {
                        hasKey = true;




                    }
                }

            }
        }

        
    }

    [PunRPC]

    public void GetPlayerColorIndex()
    {
        object playerProperty;

        if (photonView.Owner.CustomProperties.TryGetValue(Constants.PLAYER_SELETION_NUMBER, out playerProperty))
        {
            if ((int)playerProperty == 0)
            {
                playerIndex = 1;

            }
            else if ((int)playerProperty == 1)
            {
                playerIndex = 2;
            }
        }


    }

}
