using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
public class NetworkManager : MonoBehaviourPunCallbacks
{

    [Header("Login UI")]
    public GameObject LoginUIPanel;
    public InputField PlayerNameInput;

    [Header("Connecting Info Panel")]
    public GameObject ConnectingInfoUIPanel;

    [Header("Creating Room Info Panel")]
    public GameObject CreatingRoomInfoUIPanel;

    [Header("GameOptions  Panel")]
    public GameObject GameOptionsUIPanel;

    [Header("Create Room Panel")]
    public InputField RoomNameInputField;
    public string GameMode;

    [Header("Inside Room Panel")]
    public GameObject InsideRoomUIPanel;
    public Text RoomInfoText;
    public GameObject PlayerListPrefab;
    public GameObject PlayerListParent;
    public GameObject StartGameButton;
    public Text GameModeText;

    [Header("Join Random Room Panel")]

    private Dictionary<int, GameObject> playerListGameObjects;
    public List<GameObject> playerSlots = new List<GameObject>();
    public string levelName;
    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        ActivatePanel(LoginUIPanel.name);
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion

    #region UI Callback Methods
    public void OnLoginButtonClicked()
    {
        string playerName = PlayerNameInput.text;

        if (!string.IsNullOrEmpty(playerName))
        {
            ActivatePanel(ConnectingInfoUIPanel.name);

            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LocalPlayer.NickName = playerName;
                PhotonNetwork.ConnectUsingSettings();
            }
        }
        else
        {
            Debug.Log("PlayerName is invalid!");
        }
    }

    public void OnCancelButtonClicked()
    {
        ActivatePanel(GameOptionsUIPanel.name);
    }
    
    public void OnCreateRoomButtonClicked() //CREATE ROOM MARKED
    {
        CreateNewRoom();


    }
    public void OnBackButtonClicked()
    {
        ActivatePanel(GameOptionsUIPanel.name);
    }

    public void OnJoinRandomRoomClicked()
    {
        
        PhotonNetwork.JoinRandomRoom();
    }
    public void OnBlueSlimeButtonClicked()
    {
        ExitGames.Client.Photon.Hashtable newProperties = new ExitGames.Client.Photon.Hashtable()
        {
            {
                Constants.PLAYER_SELETION_NUMBER, 0

            }
        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(newProperties);
   
    }

    public void OnRedSlimeButtonClicked()
    {
        ExitGames.Client.Photon.Hashtable newProperties = new ExitGames.Client.Photon.Hashtable()
        {
            {
                Constants.PLAYER_SELETION_NUMBER, 1

            }
        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(newProperties);
        
    }
    public void OnLeaveGameButtonClicked()
    {

        ActivatePanel(GameOptionsUIPanel.name);
        ExitGames.Client.Photon.Hashtable newProperties = new ExitGames.Client.Photon.Hashtable()
        {
            {
                Constants.PLAYER_READY, false

            }
        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(newProperties);
        PhotonNetwork.LeaveRoom();
    }

    public void OnStartGameButtonClicked()
    {
       
        PhotonNetwork.LoadLevel(levelName); //CHANGE SCENE NAME MARKED
     
            
    }
    #endregion

    #region Photon Callbacks
    public override void OnConnected()
    {
        Debug.Log("Connected to Internet");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName+ " is connected to Photon");
        ActivatePanel(GameOptionsUIPanel.name);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log(PhotonNetwork.CurrentRoom + " has been created!");
    }
    
    public override void OnJoinedRoom() //inside room
    {

        ActivatePanel(InsideRoomUIPanel.name);
        
        RoomInfoText.text = "Room name: " + PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CurrentRoom.PlayerCount + " / " +
        PhotonNetwork.CurrentRoom.MaxPlayers;
      
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
           
            Photon.Realtime.Player player = PhotonNetwork.PlayerList[i];
            object currentCharacterSelection;
            if (player != PhotonNetwork.LocalPlayer)
            {
                if(player.CustomProperties.TryGetValue(Constants.PLAYER_SELETION_NUMBER, out currentCharacterSelection))
                {

                    foreach (GameObject playerSlot in playerSlots)
                    {
                        if (playerSlot.GetComponent<PlayerInitializer>().characterSelection == (int)currentCharacterSelection)
                        {
                            playerSlot.GetComponent<PlayerInitializer>().Initialize(player.ActorNumber, player.NickName);
                        }

                    }



                }
            }
            


            
       
                
        }

        







            StartGameButton.SetActive(false);
    }
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
 
        RoomInfoText.text = "Room name: " + PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CurrentRoom.PlayerCount + " / " +
        PhotonNetwork.CurrentRoom.MaxPlayers;
        StartGameButton.SetActive(CheckAllPlayerReady());

    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {

        for (int i = 0; i < playerSlots.Count; i++)
        {
            if (PhotonNetwork.LocalPlayer.NickName != playerSlots[i].GetComponent<PlayerInitializer>().PlayerNameText.text)
            {
                playerSlots[i].GetComponent<PlayerInitializer>().Deinitialize(otherPlayer.ActorNumber,otherPlayer.NickName);
                playerSlots[i].GetComponent<PlayerInitializer>().SetPlayerReady(false);
            }
             
        }
        RoomInfoText.text = "Room name: " + PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CurrentRoom.PlayerCount + " / " +
        PhotonNetwork.CurrentRoom.MaxPlayers;
        
    }

    public override void OnLeftRoom()
    {
        foreach (GameObject slot in playerSlots)
        {


            slot.GetComponent<PlayerInitializer>().isPlayerReady = false;
            slot.GetComponent<PlayerInitializer>().PlayerNameText.text = "";
            slot.GetComponent<PlayerInitializer>().PlayerNameText.gameObject.SetActive(false);
            slot.GetComponent<PlayerInitializer>().PlayerReadyButton.gameObject.SetActive(false);
            slot.GetComponent<PlayerInitializer>().chooseButton.gameObject.SetActive(true);
            slot.GetComponent<PlayerInitializer>().SetPlayerReady(false);


        }
       
        


    }

    public void CreateNewRoom()
    {
        ActivatePanel(InsideRoomUIPanel.name);

        string roomName = "Room " + Random.Range(1000, 10000);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;

        PhotonNetwork.CreateRoom(roomName, roomOptions);
        
    }

    public override void OnJoinRandomFailed(short returnCode, string message) // INSIDE ROOM MARKED ON JOIN FAILED
    {
        ActivatePanel(CreatingRoomInfoUIPanel.name);
        CreateNewRoom();
    }
   
    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        for (int i = 0; i < playerSlots.Count; i++)
        {
            object playerProperty;
            if (changedProps.TryGetValue(Constants.PLAYER_SELETION_NUMBER, out playerProperty))
            {
               // if ((int)playerProperty > 0)
                //{
                    foreach (GameObject slot in playerSlots)
                    {
                        if (slot.GetComponent<PlayerInitializer>().PlayerNameText.text == targetPlayer.NickName)
                        {

                            slot.GetComponent<PlayerInitializer>().isPlayerReady = false;
                            slot.GetComponent<PlayerInitializer>().PlayerNameText.text = "";
                            slot.GetComponent<PlayerInitializer>().PlayerNameText.gameObject.SetActive(false);
                            slot.GetComponent<PlayerInitializer>().PlayerReadyButton.gameObject.SetActive(false);
                            slot.GetComponent<PlayerInitializer>().chooseButton.gameObject.SetActive(true);
                            slot.GetComponent<PlayerInitializer>().SetPlayerReady(false);

                        }
                    }
                    //if ((int)playerProperty > 0)
                    //{
                    playerSlots[(int)playerProperty].GetComponent<PlayerInitializer>().chooseButton.gameObject.SetActive(false);
                    playerSlots[(int)playerProperty].GetComponent<PlayerInitializer>().Initialize(targetPlayer.ActorNumber, targetPlayer.NickName);
                    //}
               // }




            }
            else if (changedProps.TryGetValue(Constants.PLAYER_READY, out playerProperty))
            {
                
            

                if (playerSlots[i].GetComponent<PlayerInitializer>().PlayerNameText.text == targetPlayer.NickName) //DEINITILIZE NEEDED MARKED
                {
                    playerSlots[i].GetComponent<PlayerInitializer>().SetPlayerReady((bool)playerProperty);
                    break;
                }
                
                
            }
            
        }
        
        StartGameButton.SetActive(CheckAllPlayerReady());
    }
    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
            StartGameButton.SetActive(CheckAllPlayerReady());
        }
    }

    #endregion

    #region Public Methods
    public void ActivatePanel(string panelNameToBeActivated)
    {
        LoginUIPanel.SetActive(LoginUIPanel.name.Equals(panelNameToBeActivated));
        ConnectingInfoUIPanel.SetActive(ConnectingInfoUIPanel.name.Equals(panelNameToBeActivated));
        CreatingRoomInfoUIPanel.SetActive(CreatingRoomInfoUIPanel.name.Equals(panelNameToBeActivated));
        GameOptionsUIPanel.SetActive(GameOptionsUIPanel.name.Equals(panelNameToBeActivated));
        InsideRoomUIPanel.SetActive(InsideRoomUIPanel.name.Equals(panelNameToBeActivated));
    }

    public void SetGameMode(string gameMode)
    {
        GameMode = gameMode;
    }
    #endregion

    #region Private Methods

    private bool CheckAllPlayerReady()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return false;
        }

        foreach (Photon.Realtime.Player p in PhotonNetwork.CurrentRoom.Players.Values)
        {
            object isPlayerReady;
            if (p.CustomProperties.TryGetValue(Constants.PLAYER_READY, out isPlayerReady))
            {
                if (!(bool)isPlayerReady)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }
        return true;
    }
    #endregion
}
