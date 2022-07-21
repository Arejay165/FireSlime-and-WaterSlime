using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public enum LeverControlledInteractibleType
{

    door,
    platform,
}

public class LeverControlled : MonoBehaviourPun
{
    public GameObject go;
    public List<Lever> levers = new List<Lever>();
    public bool isInUse = true;
  

    public LeverControlledInteractibleType type;


    //Moving Platform
   
    public List<Transform> wayPointsList = new List<Transform>();
    public float currentSpeed;
    public float breakSpeed;
    public float maxSpeed;
    public int originIndex = 1;
    public int targetIndex = 0;
    public SpriteRenderer sr;

    public ConstantForce2D constantForceComponent;
    public Rigidbody2D rigidBodyComponent;
    public bool horizontalWaypoints = false;
    //very very band aid solution
    [Header("Saved Variables Absolutely do not touch")]
    public bool startingInUse;
    public Vector3 startingPosition;
    public int startingIndex;

    private void OnValidate()
    {
        //Moving Platform
        breakSpeed = Mathf.Min(0.1f, breakSpeed);//makes sure breakspeed is 0.1 so it can leave the trigger waypoint
    }
    public virtual void Start()
    {
        //Moving Platform
        //base.Start();
        go = gameObject;
       
        startingPosition = transform.position;
        startingIndex = targetIndex;
        startingInUse = isInUse;
        
        sr = GetComponent<SpriteRenderer>();
        constantForceComponent = GetComponent<ConstantForce2D>();
        rigidBodyComponent = GetComponent<Rigidbody2D>();
        if (transform.parent.childCount > 1)
        {
            foreach (Transform platformChild in transform.parent)
            {
                if (platformChild != transform)
                {
                    if (platformChild.gameObject.CompareTag("Waypoint"))
                    {
                        wayPointsList.Add(platformChild);
                    }
                }

            }
        }
        if (transform.parent.childCount > 1)
        {
            foreach (Transform leverChild in transform.parent)
            {
                if (leverChild.gameObject.GetComponent<Lever>())
                {
                    levers.Add(leverChild.gameObject.GetComponent<Lever>());
                    leverChild.gameObject.GetComponent<Lever>().controlledInteractible = this;
                }
                
                

            }
        }
        if (wayPointsList.Count > 0)
        {

            //Sets starting movement to be towards the first waypoint
            currentSpeed = maxSpeed;
            constantForceComponent.force = Vector3.Normalize(wayPointsList[targetIndex].position - transform.position) * currentSpeed;
        }


    }

    
    public void ToggleAllConnectedLevers()
    {
        foreach (Lever currentLever in levers)
        {
            currentLever.ToggleIsInUse();
        }
    }

    //Moving platform
    protected void OnTriggerEnter2D(Collider2D p_collider) //Break when arrived on target waypoint
    {
        if (wayPointsList.Count > 0)
        {
            if (wayPointsList.Contains(p_collider.transform))
            {

                if (targetIndex == wayPointsList.IndexOf(p_collider.transform))
                {
                    currentSpeed = 0;
                    int savedIndex = targetIndex;
                    originIndex = targetIndex;
                    targetIndex = savedIndex;
                    
                    constantForceComponent.force = Vector3.Normalize(wayPointsList[targetIndex].position - transform.position) * currentSpeed;
                    rigidBodyComponent.constraints = RigidbodyConstraints2D.FreezeAll;
                    
                }


            }
        }

        bool horizontalNotFrozen =  (rigidBodyComponent.constraints & RigidbodyConstraints2D.FreezePositionX) == RigidbodyConstraints2D.None;

        if (horizontalNotFrozen)
        {
            horizontalWaypoints = true;
        }

    }

   
    public void MoveToNextWaypoint(int p_targetWaypoint)
    {
        if (horizontalWaypoints)
        {
            rigidBodyComponent.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        }
        else
        {
            rigidBodyComponent.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }
        targetIndex = p_targetWaypoint;
        isInUse = isInUse ? false : true;
          
        if (wayPointsList.Count > 0)
        {
            
            if (targetIndex < wayPointsList.Count || targetIndex > 0) //If the targetted waypoint is within the waypointsList
            {
               
                currentSpeed = maxSpeed;
              
                constantForceComponent.force = Vector3.Normalize(wayPointsList[targetIndex].position - transform.position) * currentSpeed;
          

                
            }
               

            
        }
   
    }

   

    //Door
    public void ToggleDoor()
    {

        if (type == LeverControlledInteractibleType.door) //Only on or off toggle
        {
            //Lock or Toggle On
            if (isInUse) //If even number or no remainder
            {
                sr.enabled = true;
             
          
                go.layer = (int)GameManager.instance.layers[0];

            }
            //Unlock or Toggle Off
            else // If Odd number or has 1 remainder
            {
                sr.enabled = false;
               
                go.layer = (int)GameManager.instance.layers[3];
         

            }

        }
  
    }

 
}
