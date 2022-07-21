using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using UnityEngine.Tilemaps;


public enum ReactionType
{
    
    intervalColorSwitch,
    intervalColorChange,
    touchColorSwitch,
    staticColorChange,
}
public class Interactible : MonoBehaviourPun
{

    public ReactionType reactionType;


    public float interval;
    [SerializeField] public bool canChangeColor;


    public bool canReact;
    public SpriteRenderer spriteRendererComponent;
    public int colorIndex;
    public int layerIndex;
    public Tilemap tilemap;
    public bool isNeutral;
    public virtual void Start()
    {
        interval = Mathf.Max(0.1f, interval);
        spriteRendererComponent = GetComponent<SpriteRenderer>();

        int matchingLayerIndex = 0;
        for (int i = 0; i < GameManager.instance.layers.Count; i++)
        {
            if ((int)GameManager.instance.layers[i] == gameObject.layer)
            {
                matchingLayerIndex = i;
                break;
            }
        }
       
        int matchingColorIndex = matchingLayerIndex;
        if (gameObject.layer == GameManager.instance.layers.Count + 6)
        {
            matchingColorIndex = Random.Range(3, GameManager.instance.interactionColors.Count);
            
        }
        if (canChangeColor)
        {

            if(spriteRendererComponent != null)
            {
                spriteRendererComponent.color = GameManager.instance.interactionColors[matchingColorIndex];
            }
        
            if (tilemap != null)
            {
                tilemap.color = GameManager.instance.interactionColors[matchingColorIndex];
            }
        }



        if (tilemap != null)
        {
            tilemap.color = Color.gray;
            Debug.Log("HasTilemap");
        }

        //Initialize Random seed
        Random.InitState(42);
        

    }


    
    public void DoInteraction(int p_targetObjectID)
    {
        if (photonView.IsMine)
        {
            photonView.RPC("DoFunction", RpcTarget.AllBuffered, p_targetObjectID);
        }
        
    
    }

    [PunRPC]
    protected virtual void DoFunction(int p_targetObjectID)
    {
        if (photonView.IsMine)
        {
            if (canReact)
            {

                // DoReaction(p_targetObjectID);
                photonView.RPC("DoReaction", RpcTarget.AllBuffered, p_targetObjectID);
            }
        }


    }
    [PunRPC]
    protected void DoReaction(int p_targetObjectID)
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            
            if (reactionType == ReactionType.intervalColorSwitch)
            {
                IntervalColorSwitch(p_targetObjectID);
            }
            else if (reactionType == ReactionType.intervalColorChange)
            {
                IntervalColorChange(p_targetObjectID);
            }
            else if (reactionType == ReactionType.touchColorSwitch)
            {
                photonView.RPC("TouchColorSwitch", RpcTarget.AllBuffered, p_targetObjectID);
           
            }else if (reactionType == ReactionType.staticColorChange)
            {
                photonView.RPC("StaticColorChange", RpcTarget.AllBuffered, p_targetObjectID);
            }
        }
       
        
    }

    [PunRPC]
    public void StaticColorChange(int index)
    {
        Debug.Log(index);
        if (spriteRendererComponent != null)
        {
            spriteRendererComponent.color = GameManager.instance.interactionColors[index];
        }
    

        if (tilemap != null)
        {
            Debug.Log(index);
            tilemap.color = GameManager.instance.interactionColors[index];
        }
        gameObject.layer = (int)GameManager.instance.layers[index];

    
    }
   
    
    [PunRPC]
    protected void RPCColorChange()
    {

        int lastLayer = gameObject.layer;
        //Make sure next color chosen isn't the same as the last one 
        while (lastLayer == gameObject.layer)
        {
            int chosenIndex = Random.Range(0, GameManager.instance.interactionColors.Count);

            if(spriteRendererComponent != null)
            {
                spriteRendererComponent.color = GameManager.instance.interactionColors[chosenIndex];
            }
  

            if(tilemap != null)
            {
                tilemap.color = GameManager.instance.interactionColors[chosenIndex];
            }
            //If color chosen is not a player color or a neutral one, it's layer is none 
            if (chosenIndex > 3)
            {
                lastLayer = (int)GameManager.instance.layers[3];
                
            }
            else
            {
                lastLayer = (int)GameManager.instance.layers[chosenIndex];
                
            }
            
        }
        gameObject.layer = lastLayer;




    }


    [PunRPC]
    protected void RPCColorSwitch()
    {

        LayerMask chosenLayer = gameObject.layer;
        int chosenIndex = 0;
        
        //Make sure next color chosen isn't the same as the last one 
        while (chosenLayer == gameObject.layer)
        {
            chosenIndex = Random.Range(1, 3);
            chosenLayer = (int)GameManager.instance.layers[chosenIndex];



        }
        
        gameObject.layer = (int)chosenLayer;

        if(spriteRendererComponent != null)
        {

            spriteRendererComponent.color = GameManager.instance.interactionColors[chosenIndex];

        }
        if (tilemap != null)
        {
            tilemap.color = GameManager.instance.interactionColors[chosenIndex];
        }
    }
   

    protected virtual void IntervalColorSwitch(int p_targetObjectID)
    {
        canReact = false;
        Coroutine newCoroutine = StartCoroutine(ColorSwitchTimer(p_targetObjectID,interval));

        
    }

    public virtual void IntervalColorChange(int p_targetObjectID)
    {
        canReact = false;
        Coroutine newCoroutine = StartCoroutine(ColorChangeTimer(p_targetObjectID,interval));
    }

    [PunRPC]
    protected virtual void TouchColorSwitch(int p_targetObjectID)
    {
        LayerMask chosenLayer = gameObject.layer;
        int chosenIndex = 0;
        canReact = false;
        //Make sure next color chosen isn't the same as the last one 
        while (chosenLayer == gameObject.layer)
        {
            chosenIndex = Random.Range(1, 3);
            chosenLayer = (int)GameManager.instance.layers[chosenIndex];
        }

        gameObject.layer = (int)chosenLayer;

        if(spriteRendererComponent != null)
        {
            spriteRendererComponent.color = GameManager.instance.interactionColors[chosenIndex];

        }

        if (tilemap != null)
        {
            tilemap.color = GameManager.instance.interactionColors[chosenIndex];
        }
        StartCoroutine(ColorSwitchCooldown());


    }

    [PunRPC]
    protected virtual void TouchColorSwitchReset()
    {
       
        canReact = true;
    }

    protected IEnumerator ColorChangeTimer(int p_targetObjectID,float p_time = 1.0f)
    {
        
        yield return new WaitForSeconds(p_time);
        

        if (canChangeColor)
        {
            
            photonView.RPC("RPCColorChange", RpcTarget.AllBuffered);
        }
     
        Coroutine newCoroutine = StartCoroutine(ColorChangeTimer(p_targetObjectID,p_time));

    }

    protected IEnumerator ColorSwitchTimer(int p_targetObjectID,float p_time = 1.0f)
    {
        yield return new WaitForSeconds(p_time);
        
        

        if (canChangeColor)
        {
            photonView.RPC("RPCColorSwitch", RpcTarget.AllBuffered);
         
        }
  
        Coroutine newCoroutine = StartCoroutine(ColorSwitchTimer(p_targetObjectID,p_time));
    }

    protected IEnumerator ColorSwitchCooldown(float p_time = 1.0f)
    {
        yield return new WaitForSeconds(p_time);

        if (canChangeColor)
        {
            photonView.RPC("TouchColorSwitchReset", RpcTarget.AllBuffered);

        }
    }

    public void ResetTiles()
    {
        gameObject.layer = 0;
        tilemap.color = Color.gray;
        isNeutral = true;
    }
}
