using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
public class Key : Interactible
{

    [PunRPC]
    protected override void DoFunction(int p_targetObjectID)
    {
        if (PhotonView.Find(p_targetObjectID).gameObject.GetComponent<Player>().hasKey == false)
        {
            PhotonView.Find(p_targetObjectID).gameObject.GetComponent<Player>().hasKey = true;
            //keyTaken();
            photonView.RPC("keyTaken", RpcTarget.AllBuffered);
        }
            
        
      
    }

    [PunRPC]
    public void keyTaken()
    {
        gameObject.SetActive(false);

    }
}
