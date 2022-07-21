using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
public class Hazard : Interactible
{
    public float damage;
    public override void Start()
    {
        //base.Start();

        damage = 1;
    }

    [PunRPC]
    protected override void DoFunction(int p_targetObjectID)
    {
        base.DoFunction(p_targetObjectID);
        if (photonView.IsMine)
        {
            if (PhotonView.Find(p_targetObjectID).gameObject.GetComponent<Health>())
            {
                PhotonView.Find(p_targetObjectID).gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, damage);

            }
        }
        
    }

}
