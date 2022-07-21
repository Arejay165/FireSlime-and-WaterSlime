using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using Photon.Pun;
public class AccessGate : Interactible
{
    // Start is called before the first frame update
    public GameObject disableLock;
    public bool hasLock = true;
    public bool isTypeDoor;
    public GameObject playformWaypoints;
    [PunRPC]
    protected override void DoFunction(int p_targetObjectID)
    {
        Debug.Log(p_targetObjectID);

        if (canReact)
        {
            //Unlock
            if (hasLock)
            {
                //Door
                if (isTypeDoor)
                {
                    disableLock.SetActive(true);
                    disableLock.GetComponent<BoxCollider2D>().isTrigger = false;
                    spriteRendererComponent.color = GameManager.instance.interactionColors[colorIndex];
                    disableLock.transform.root.gameObject.GetComponent<SpriteRenderer>().color = GameManager.instance.interactionColors[colorIndex];
                    disableLock.GetComponent<SpriteRenderer>().color = GameManager.instance.interactionColors[colorIndex];
                    disableLock.layer = (int)GameManager.instance.layers[colorIndex];
                    hasLock = false;
                }
                //Platform
                else
                {
                    disableLock.SetActive(false);
                    disableLock.GetComponent<BoxCollider2D>().isTrigger = false;
                    spriteRendererComponent.color = GameManager.instance.interactionColors[colorIndex];
                    disableLock.transform.root.gameObject.GetComponent<SpriteRenderer>().color = GameManager.instance.interactionColors[colorIndex];
                    disableLock.GetComponent<SpriteRenderer>().color = GameManager.instance.interactionColors[colorIndex];
                    disableLock.layer = (int)GameManager.instance.layers[colorIndex];
                    hasLock = true;
                }
            }
            //Lock
            else
            {
                //Door
                if (isTypeDoor)
                {
                    disableLock.SetActive(false);
                    disableLock.GetComponent<BoxCollider2D>().isTrigger = true;
                    spriteRendererComponent.color = GameManager.instance.interactionColors[colorIndex];
                    disableLock.transform.root.gameObject.GetComponent<SpriteRenderer>().color = GameManager.instance.interactionColors[colorIndex];
                    disableLock.GetComponent<SpriteRenderer>().color = GameManager.instance.interactionColors[colorIndex];
                    disableLock.layer = (int)GameManager.instance.layers[colorIndex];
                    hasLock = true;
                }
                //Platform
                else
                {

                }
             
            }
        }
        else
        {
            if (canReact)
            {
                
                disableLock.SetActive(true);
                disableLock.GetComponent<BoxCollider2D>().isTrigger = false;
                hasLock = false;
            }

        }



    }
}