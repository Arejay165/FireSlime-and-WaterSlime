using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
public class Door : Interactible
{

    // Start is called before the first frame update

    public override void Start()
    {
        base.Start();
       
    }

    [PunRPC]
    protected override void DoFunction(int p_targetObjectID)
    {
        //if (PhotonView.Find(p_targetObjectID).gameObject.GetComponent<Player>().hasKey)
        //{
        //    PhotonView.Find(p_targetObjectID).RPC("FinishedLevel", RpcTarget.AllBuffered);
        //}
        Debug.Log(p_targetObjectID);

        if (isNeutral)
        {

         //   int index = GameManager.instance.playerFounded[p_targetObjectID].gameObject.GetComponent<PhotonView>().ViewID;
           // Debug.Log(index);
            base.DoFunction(p_targetObjectID);
            isNeutral = false;
            if (GameManager.instance.playerFounded[p_targetObjectID].gameObject.GetComponent<Player>().hasKey)
            {
                // PhotonView.Find(p_targetObjectID).RPC("FinishedLevel", RpcTarget.AllBuffered);
                GameManager.instance.playerFounded[p_targetObjectID ].gameObject.GetComponent<PhotonView>().RPC("FinishedLevel", RpcTarget.AllBuffered);
                AudioManager.instance.Play("EnterDoor"); //[AUDIO TO FILL UP] // 
            }
        }
        else
        {
            if (PhotonView.Find(p_targetObjectID).gameObject.GetComponent<Player>().hasKey)
            {
                PhotonView.Find(p_targetObjectID).RPC("FinishedLevel", RpcTarget.AllBuffered);
                AudioManager.instance.Play("EnterDoor"); //[AUDIO TO FILL UP] // 
            }
        }

    }

}
   

