using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using Photon.Pun;
public class Lever : Interactible
{
    // Start is called before the first frame update
    public LeverControlled controlledInteractible;
    public bool isInUse = true;
    public int originalState = 0;
    public int targetState = 0;

    public bool savedIsInUse;

    public override void Start()
    {
        base.Start();
        savedIsInUse = isInUse;
    }


    [PunRPC]
    protected override void DoFunction(int p_targetObjectID)
    {

  
        controlledInteractible.isInUse = controlledInteractible.isInUse ? false : true;

        
        if (controlledInteractible.type == LeverControlledInteractibleType.door)
        {
                            
            controlledInteractible.ToggleDoor();
        }
        else if (controlledInteractible.type == LeverControlledInteractibleType.platform)
        {

            if (isInUse)
            {
                AudioManager.instance.Play("LeverOn"); //[AUDIO TO FILL UP] // open sound
                controlledInteractible.originIndex = originalState;
                controlledInteractible.targetIndex = targetState ;
                controlledInteractible.MoveToNextWaypoint(targetState);
                
            }
            else
            {
                AudioManager.instance.Play("LeverOff"); //[AUDIO TO FILL UP] // close sound
                controlledInteractible.originIndex = targetState;
                controlledInteractible.targetIndex = originalState;
                
                controlledInteractible.MoveToNextWaypoint(originalState);
            }
                
        }
      
        controlledInteractible.ToggleAllConnectedLevers();
        
       
        
    }

    
    public void ToggleIsInUse()
    {

        isInUse = isInUse ? false : true;
        if (isInUse)
        {
            AudioManager.instance.Play("LeverOn"); //[AUDIO TO FILL UP] // open sound
        }
        else
        {
            AudioManager.instance.Play("LeverOff"); //[AUDIO TO FILL UP] // close sound
        }
        



    }

 

    
}