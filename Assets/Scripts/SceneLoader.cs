using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;
public class SceneLoader : MonoBehaviourPunCallbacks

{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public override void OnConnectedToMaster()
    {

        PhotonNetwork.AutomaticallySyncScene = true;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoToNextScene(string levelName)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(levelName);
        }

    }
}
