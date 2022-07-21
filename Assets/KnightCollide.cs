using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class KnightCollide : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    public Animator anim;
    void Start()
    {
       // anim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            photonView.RPC("CelebrateCollide", RpcTarget.AllBuffered, true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            photonView.RPC("CelebrateCollide", RpcTarget.AllBuffered, false);
        }
    }

    [PunRPC]

    public void CelebrateCollide(bool value)
    {
        anim.SetBool("Jump", value);
    }
}
