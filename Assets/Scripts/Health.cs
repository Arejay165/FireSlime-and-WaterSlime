using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;
public class Health : MonoBehaviourPun
{
    public bool isAlive;
    public float currentHealth;
    public float maxHealth;
    private Player playerRef;
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        playerRef = gameObject.GetComponent<Player>();
    }

    public void Initialize(float p_maxHealth)
    {
        isAlive = true;
        maxHealth = p_maxHealth;
        currentHealth = maxHealth;

        

    }


    [PunRPC]
    public void TakeDamage(float p_amount)
    {
        //float p_amount, string p_source, int p_viewID, int p_killerViewID)
        currentHealth -= p_amount;
        
        if (isAlive)
        {
            
            currentHealth -= p_amount;
            if (currentHealth <= 0)
            {
                //  Death();
                if (photonView.IsMine)
                {
                    photonView.RPC("Death", RpcTarget.AllBuffered);
                }
            }
        }



    }


    [PunRPC]
    void Death()
    {
        isAlive = false;

        AudioManager.instance.Play("SlimeDeath"); //[AUDIO TO FILL UP]
        playerRef.canMove = false;
        playerRef.canJump = false;
        playerRef.sr.enabled = false;
        playerRef.rb.velocity = new Vector2(0, 0);
        playerRef.bc.isTrigger = true;
        playerRef.rb.constraints = RigidbodyConstraints2D.FreezeAll;
        playerRef.attachedToGround = null;
        if (photonView.IsMine)
        {
            
            StartCoroutine(delay());
        }
       
       // GameManager.instance.ResetTiles();
        currentHealth = maxHealth;
        
    }
    IEnumerator delay()
    {
        yield return new WaitForSeconds(3f);
      
        
        GameManager.instance.GetComponent<PhotonView>().RPC("ResetAll", RpcTarget.AllBuffered);
        
        
    }
    [PunRPC]
    IEnumerator deathAnimation()
    {
        playerRef.anim.SetBool("IsDeath", true);
        yield return new WaitForSeconds(1.5f);
        playerRef.anim.SetBool("IsDeath", false);
    }

 
}
