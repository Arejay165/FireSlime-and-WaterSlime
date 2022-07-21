using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class PlayerInitializer : MonoBehaviour
{
    [Header("UI References")]
    public Text PlayerNameText;
    public Button PlayerReadyButton;
    public Image PlayerReadyImage;
    public Button chooseButton;
    public bool isOccupied = false;
    public bool isPlayerReady = false;
    public bool listenerAdded = false;
    public int characterSelection;
    public void Initialize(int playerId, string playerName)
    {
        isOccupied = true;
        PlayerNameText.text = playerName;
        PlayerNameText.gameObject.SetActive(true);
        PlayerReadyButton.gameObject.SetActive(true);
        
        object currentIsPlayerReady;
        if (PhotonNetwork.CurrentRoom.GetPlayer(playerId).CustomProperties.TryGetValue(Constants.PLAYER_READY, out currentIsPlayerReady))
        {
            SetPlayerReady((bool)currentIsPlayerReady);
        }
        if (PhotonNetwork.LocalPlayer.ActorNumber != playerId)
        {
            chooseButton.gameObject.SetActive(false);
            PlayerReadyButton.gameObject.SetActive(false);
        
        }
        else
        {

            //sets custom property for each player "isPlayerReady
            
            ExitGames.Client.Photon.Hashtable initializeProperties = new ExitGames.Client.Photon.Hashtable()
            {
                {
                    Constants.PLAYER_READY, isPlayerReady
                    
                }
            };
            
            PhotonNetwork.LocalPlayer.SetCustomProperties(initializeProperties);
            if (!listenerAdded)
            {
                listenerAdded = true;
                PlayerReadyButton.onClick.AddListener(() =>
                {
                    isPlayerReady = !isPlayerReady;
                    
                    SetPlayerReady(isPlayerReady);
                    ExitGames.Client.Photon.Hashtable newProperties = new ExitGames.Client.Photon.Hashtable()
                    {
                        {
                            Constants.PLAYER_READY, isPlayerReady

                        }
                    };
                    
                    PhotonNetwork.LocalPlayer.SetCustomProperties(newProperties);
                });
            }
      
        }
    }
    public void Deinitialize(int playerId, string playerName)
    {
        Debug.Log("Deinitizligin");
        isOccupied = false;
        isPlayerReady = false;
        PlayerNameText.text = "";
        PlayerNameText.gameObject.SetActive(false);
        PlayerReadyButton.gameObject.SetActive(false);
        chooseButton.gameObject.SetActive(true);
        if (PhotonNetwork.LocalPlayer.ActorNumber == playerId)
        {
  
            ExitGames.Client.Photon.Hashtable initializeProperties = new ExitGames.Client.Photon.Hashtable()
            {
                {
                Constants.PLAYER_READY, false

                },
                {

                Constants.PLAYER_SELETION_NUMBER,0
                }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(initializeProperties);
            SetPlayerReady(false);
        }
        
    }
    public void SetPlayerReady(bool p_playerReady)
    {
        PlayerReadyImage.enabled = p_playerReady;

        if (p_playerReady)
        {
            PlayerReadyButton.GetComponentInChildren<Text>().text = "Ready!";
        }
        else
        {
            PlayerReadyButton.GetComponentInChildren<Text>().text = "Ready?";
        }
    }
}
