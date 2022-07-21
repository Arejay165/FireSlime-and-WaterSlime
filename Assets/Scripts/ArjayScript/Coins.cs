using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
public class Coins : Interactible
{
  
    [PunRPC]
    protected override void DoFunction(int p_targetObjectID)
    {
        if (photonView.IsMine)
        {
            PhotonView.Find(p_targetObjectID).RPC("AddCoins", RpcTarget.AllBuffered);
        
            photonView.RPC("coinTaken", RpcTarget.AllBuffered);
        }
        //        coinTaken();
    }

    [PunRPC]
    public void coinTaken()
    {
        gameObject.SetActive(false);

    }
}
