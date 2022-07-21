using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using TMPro;

public class TextTrigger : Interactible
{

    public GameObject tutorialText;
    public override void Start()
    {
        base.Start();
        if (tutorialText)
        {
            tutorialText.SetActive(false);
        }
    }
    [PunRPC]
    protected override void DoFunction(int p_targetObjectID)
    {
        if (GameManager.instance.tutorialTextTriggers.Count > 0)
        { 
            foreach(TextTrigger tutorialTextTrigger in GameManager.instance.tutorialTextTriggers)
            {
                if (tutorialTextTrigger == this)
                {
                    if (tutorialTextTrigger.tutorialText.gameObject != null)
                    {
                        tutorialTextTrigger.tutorialText.SetActive(true);
                    }
                }
                else
                {
                    if (tutorialTextTrigger.tutorialText.gameObject != null)
                    {
                        tutorialTextTrigger.tutorialText.SetActive(false);
                    }
                }
                
            }
        
         
        }    
      



    }

}
