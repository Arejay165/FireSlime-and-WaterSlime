using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;
[System.Serializable]
public enum LayerTest
{
    //numbers after the 7 default layers
    all = 7,
    blue = 8,
    red = 9,
    none = 10,    
}
public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;
    public bool isPlaying = true;

    private int playersInGame = 0;

    public Text winnerText;
    public bool blueFinished;
    public bool redFinished;

    public int blueCoin = 0;
    public int redCoin = 0;
    public int maxBlueCoins = 0;
    public int maxRedCoins = 0;
    public PlayerGUI plrUI;

    public GameObject[] slimes;
    public List<Transform> spawnPoints = new List<Transform>();

    public List<LayerTest> layers = new List<LayerTest>();
    public List<Color> interactionColors = new List<Color>();
    public string currentLevel;
    public string nextLevel;
    public bool hasCutscene;
    public bool noNeedKey;
    private PhotonView PhotonView;
    public List<GameObject> InteractableItems;
    public List<GameObject> movingObjects;
    public List<GameObject> playerFounded;
    public List<TextTrigger> tutorialTextTriggers = new List<TextTrigger>();
    public List<GameObject> neutralPlatforms;
    public Sprite[] slimeSprites;

    // Start is called before the first frame update
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            PhotonView = GetComponent<PhotonView>();
        }
        else
        {
            Destroy(gameObject);
        }
        SceneManager.sceneLoaded += OnSceneFinishedLoading;//callback if the scene finishes loading
    }
    
    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game")
        {
            if (PhotonNetwork.IsMasterClient)
            {
                MasterLoadedGame();
            }
            else
            {
                NonMasterLoadedGame();
            }
        }
    }
    
    public void MasterLoadedGame()
    {
        playersInGame = 1;
        PhotonView.RPC("RPC_LoadGameOthers", RpcTarget.Others);
    }

    private void NonMasterLoadedGame()
    {
        PhotonView.RPC("RPC_LoadedGameScene", RpcTarget.MasterClient);
    }

    [PunRPC]
    private void RPC_LoadGameOthers()
    {
        PhotonNetwork.LoadLevel(nextLevel);
    }

    [PunRPC]
    public void StartVotePause()
    {
        plrUI.pauseVote.SetActive(true);

    }

    [PunRPC]
    public void StartVoteUnpause()
    {
        plrUI.unpauseVote.SetActive(true);

    }

    [PunRPC]
    public void StartVoteRestart()
    {
        plrUI.restartVote.SetActive(true);
    }

    [PunRPC]
    public void StartVoteQuit()
    {
        plrUI.quitVote.SetActive(true);
        
    }

    [PunRPC]
    public void PauseGame()
    {
        plrUI.YesButtonClicked();
        isPlaying = false;
        plrUI.pauseVote.SetActive(false);
    }

    [PunRPC]
    public void PauseGameFailed()
    {
        plrUI.NoButtonClicked();
        plrUI.pauseVote.SetActive(false);
    }

    [PunRPC]
    public void UnpauseGame()
    {
        plrUI.YesButtonClicked();
        isPlaying = true;
        plrUI.unpauseVote.SetActive(false);
    }

    [PunRPC]
    public void UnpauseGameFailed()
    {
        plrUI.NoButtonClicked();
        plrUI.unpauseVote.SetActive(false);
    }

    [PunRPC]
    public void RestartGame()
    {
        plrUI.YesButtonClicked();
        plrUI.restartVote.SetActive(false);
        if (photonView.IsMine)
        {
            GameManager.instance.photonView.RPC("ResetAll", RpcTarget.AllBuffered);
        }
        
    }
    
    [PunRPC]
    public void RestartGameFailed()
    {
        plrUI.NoButtonClicked();
        plrUI.restartVote.SetActive(false);
    }



    [PunRPC]
    public void QuitGame()
    {
        plrUI.YesButtonClicked();
        plrUI.quitVote.SetActive(false);
        if (photonView.IsMine)
        {
            PhotonNetwork.LeaveRoom();
            
            PhotonNetwork.LoadLevel("LobbyScene");
        }
    }

    [PunRPC]
    public void QuitGameFailed()
    {
        plrUI.NoButtonClicked();
        plrUI.quitVote.SetActive(false);
    }
    [PunRPC]
    private void RPC_LoadedGameScene()
    {
        playersInGame++;
        if (playersInGame == PhotonNetwork.PlayerList.Length)
        {
            print("All players are in the game scene. ");
        }
    }

    private void Start()
    {
        Initialize();
        //Add all spawn points
        foreach (GameObject foundSpawnPoint in GameObject.FindGameObjectsWithTag("SpawnPoint"))
        {

            spawnPoints.Add(foundSpawnPoint.transform);
            
        }

        if (PhotonNetwork.IsConnectedAndReady)
        {
            object playerSelectionNumber;

            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(Constants.PLAYER_SELETION_NUMBER, out playerSelectionNumber))
            {
               
                Vector3 instantiatePosition = spawnPoints[(int)playerSelectionNumber].position;
                GameObject newSlimeInstance = PhotonNetwork.Instantiate(slimes[(int)playerSelectionNumber].name, instantiatePosition, Quaternion.identity);
                PhotonNetwork.LocalPlayer.TagObject = newSlimeInstance;
                

            }

        }
   
        //Adding tutorial text triggers
        foreach (GameObject foundInteractible in GameObject.FindGameObjectsWithTag("Interactible"))
        {
            if (foundInteractible.GetComponent<TextTrigger>())
            {
                tutorialTextTriggers.Add(foundInteractible.GetComponent<TextTrigger>());
            }
            else if (foundInteractible.GetComponent<LeverControlled>() || foundInteractible.GetComponent<Lever>())
            {
                movingObjects.Add(foundInteractible.gameObject);
            }
            else if (foundInteractible.GetComponent<Coins>())
            {
                if (foundInteractible.layer == LayerMask.NameToLayer("Blue"))
                {
                    maxBlueCoins++;
                }
                else if (foundInteractible.layer == LayerMask.NameToLayer("Red"))
                {
                    maxRedCoins++;

                }
                InteractableItems.Add(foundInteractible);
            }
            else if (foundInteractible.GetComponent<MovingPlatform>())
            {

                movingObjects.Add(foundInteractible.gameObject);
            }
            else if (foundInteractible.GetComponent<Platform>())
            {
                if (foundInteractible.GetComponent<Platform>().reactionType == ReactionType.staticColorChange)
                {
                    neutralPlatforms.Add(foundInteractible);
                }
            }
            else if (foundInteractible.GetComponent<Door>())
            {
                if (foundInteractible.GetComponent<Door>().reactionType == ReactionType.staticColorChange)
                {
                    neutralPlatforms.Add(foundInteractible);
                }
            }

            else
            {
                InteractableItems.Add(foundInteractible);
            }
        }
        if (noNeedKey)
        {
            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            {
                player.GetComponent<Player>().hasKey = true;
            }
        }

    

    }

    public void Initialize()
    {
        GameManager.instance.isPlaying = true;
        winnerText.gameObject.SetActive(false);

    }

    [PunRPC]
    public void ResetAll()
    {
        isPlaying = true;
        foreach (GameObject go in InteractableItems)
        {
            go.SetActive(true);
         
        }

        foreach(GameObject player in playerFounded)
        {
            
            player.gameObject.GetComponent<Player>().hasKey = false;
        }
        redFinished = false;
        blueFinished = false;
        
        blueCoin = 0;
        redCoin = 0;
        if (photonView.IsMine)
        {
            photonView.RPC("RespawnMovingObjects", RpcTarget.AllBuffered);
            photonView.RPC("RespawnLocation", RpcTarget.AllBuffered);
            photonView.RPC("ResetTiles", RpcTarget.AllBuffered);
        }
        

    }
    [PunRPC]
    public void RespawnMovingObjects()
    {
        // playerPos.position

        foreach (GameObject go in movingObjects)
        {
            if (go.GetComponent<LeverControlled>())
            {
                go.GetComponent<LeverControlled>().isInUse = go.GetComponent<LeverControlled>().startingInUse;
                go.GetComponent<LeverControlled>().targetIndex = go.GetComponent<LeverControlled>().startingIndex;
                go.GetComponent<LeverControlled>().currentSpeed = 0;
                
                if (go.GetComponent<LeverControlled>().type == LeverControlledInteractibleType.platform)
                {
                    go.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                    go.GetComponent<ConstantForce2D>().force = new Vector2(0f, 0f);

                    if (go.GetComponent<LeverControlled>().wayPointsList.Count > 0)
                    {
                        if (go.GetComponent<LeverControlled>().horizontalWaypoints)
                        {
                            go.GetComponent<LeverControlled>().rigidBodyComponent.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                        }
                        else
                        {
                            go.GetComponent<LeverControlled>().rigidBodyComponent.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                        }
                        //Sets starting movement to be towards the first waypoint
                        go.GetComponent<LeverControlled>().currentSpeed = go.GetComponent<LeverControlled>().maxSpeed;
                        
                        go.GetComponent<ConstantForce2D>().force = Vector3.Normalize(go.GetComponent<LeverControlled>().wayPointsList[go.GetComponent<LeverControlled>().targetIndex].position - transform.position) * go.GetComponent<LeverControlled>().currentSpeed;
                       
                    }
                }
                else if (go.GetComponent<LeverControlled>().type == LeverControlledInteractibleType.door)
                {
                    go.GetComponent<LeverControlled>().ToggleDoor();

                }
                go.transform.position = go.GetComponent<LeverControlled>().startingPosition;

            }
            else if (go.GetComponent<Lever>())
            {
                go.GetComponent<Lever>().isInUse = go.GetComponent<Lever>().savedIsInUse;
            }
            else if(go.GetComponent<MovingPlatform>())
            {
                go.GetComponent<MovingPlatform>().isAscendingWaypointIndex = go.GetComponent<MovingPlatform>().startingIsAscending;
                go.GetComponent<MovingPlatform>().currentWaypointIndex = go.GetComponent<MovingPlatform>().startingIndex;

                go.GetComponent<MovingPlatform>().currentSpeed = 0;
                go.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                go.GetComponent<ConstantForce2D>().force = new Vector2(0f, 0f);

                if (go.GetComponent<MovingPlatform>().wayPointsList.Count > 0)
                {
                    if (go.GetComponent<MovingPlatform>().horizontalWaypoints)
                    {
                        go.GetComponent<MovingPlatform>().rigidBodyComponent.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                    }
                    else
                    {
                        go.GetComponent<MovingPlatform>().rigidBodyComponent.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                    }

                    //Sets starting movement to be towards the first waypoint
                    go.GetComponent<MovingPlatform>().currentSpeed = go.GetComponent<MovingPlatform>().maxSpeed;
                   
                    go.GetComponent<ConstantForce2D>().force = (go.GetComponent<MovingPlatform>().wayPointsList[go.GetComponent<MovingPlatform>().currentWaypointIndex].position - transform.position).normalized  * go.GetComponent<MovingPlatform>().currentSpeed;
               
                }
                go.transform.position = go.GetComponent<MovingPlatform>().startingPosition;

            }
            
        }
        

    }
    [PunRPC]
    public void RespawnLocation()
    {
        for(int i = 0; i < playerFounded.Count; i++)
        {

            object playerSelectionNumber;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(Constants.PLAYER_SELETION_NUMBER, out playerSelectionNumber))
            {
                playerFounded[i].gameObject.GetComponent<Player>().bc.isTrigger = false;
                playerFounded[i].gameObject.GetComponent<Player>().rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                playerFounded[i].gameObject.GetComponent<Player>().attachedToGround = null;
                playerFounded[i].gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                
                playerFounded[i].transform.position = spawnPoints[(int)playerSelectionNumber].position;
             
                playerFounded[i].gameObject.GetComponent<Player>().sr.enabled = true;
                playerFounded[i].gameObject.GetComponent<Player>().canMove = true;
                playerFounded[i].gameObject.GetComponent<Player>().canJump = true;
                
                playerFounded[i].gameObject.GetComponent<Health>().Initialize(playerFounded[i].gameObject.GetComponent<Health>().maxHealth);
                
            }

        }

    }

    [PunRPC]
    public void ResetTiles()
    {

        if (neutralPlatforms != null)
        {
            foreach (GameObject go in neutralPlatforms)
            {
                if (go.gameObject.GetComponent<Platform>())
                {
                    go.gameObject.GetComponent<Platform>().ResetTiles();
                }

            }
        }



    }

    //public void enableMovement()
    //{
    //    foreach(GameObject go in playerFounded)
    //    {
    //        go.gameObject.GetComponent<Player>().canMove = true;
    //        go.gameObject.gameObject.GetComponent<Player>().canJump = true;
    //        go.gameObject.GetComponent<Player>().hasKey = true;
    //    }
    //}

    public void hidePlayer()
    {

        for (int i = 0; i < playerFounded.Count; i++)
        {
   
            playerFounded[i].GetComponent<SpriteRenderer>().enabled = false;
            Debug.Log(playerFounded[i].GetComponent<SpriteRenderer>().enabled);
            playerFounded[i].gameObject.GetComponent<Player>().canMove = false;
            playerFounded[i].gameObject.GetComponent<Player>().canJump = false;
            //   playerFounded[i].SetActive()
        }
    }

    public void showPlayer()
    {

        for (int i = 0; i < playerFounded.Count; i++)
        {
       
            playerFounded[i].GetComponent<SpriteRenderer>().enabled = true;
            Debug.Log(playerFounded[i].GetComponent<SpriteRenderer>().enabled);
            playerFounded[i].GetComponent<Player>().canMove = true;
            playerFounded[i].gameObject.GetComponent<Player>().canJump = true;
            playerFounded[i].GetComponent<Player>().hasKey = true;
            //   playerFounded[i].SetActive()
        }
    }
}
