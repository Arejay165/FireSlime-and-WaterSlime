using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
public class Platform : Interactible
{
  


    [PunRPC]
    protected override void DoFunction(int p_targetObjectID)
    {
        if (isNeutral)
        {
            base.DoFunction(p_targetObjectID);
            isNeutral = false;
        }

      
    }



}


