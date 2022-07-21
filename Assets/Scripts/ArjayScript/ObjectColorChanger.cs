using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
public class ObjectColorChanger : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update

   
    public bool isNeutral;
    public SpriteRenderer sprite;


    void Start()
    {
        sprite = this.gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player")  && this.isNeutral)
        {
            //Change this to RPC
             ChangeTileColors(collision.gameObject);
           // photonView.RPC("ChangeTileColors", RpcTarget.AllBuffered, collision.gameObject);
        }
    }

    [PunRPC]
    public void ChangeTileColors(GameObject player)
    {
        gameObject.layer = player.layer;
        isNeutral = false;
        sprite.color = player.gameObject.GetComponent<PlayerMovement>().playerColor;
    }

}
