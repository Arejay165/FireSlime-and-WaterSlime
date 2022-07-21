using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class MovingPlatform : Interactible
{
    public int currentWaypointIndex = 0;
    public List<Transform> wayPointsList = new List<Transform>();
    public float currentSpeed;
    public float maxSpeed;
    public bool isAscendingWaypointIndex = false;

    public ConstantForce2D constantForceComponent;
    public Rigidbody2D rigidBodyComponent;
    public bool horizontalWaypoints = false;
    //very very band aid solution
    [Header("Saved Variables Absolutely do not touch")]
    public bool startingIsAscending = false;
    public Vector3 startingPosition = new Vector3(0,0,0);
    public int startingIndex = 0;

    public override void Start()
    {
        base.Start();
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
        if (wayPointsList.Count > 0)
        {

            //Sets starting movement to be towards the first waypoint
            currentSpeed = maxSpeed;
            constantForceComponent.force = (wayPointsList[currentWaypointIndex].position - transform.position).normalized * currentSpeed;
            startingIsAscending = isAscendingWaypointIndex;
            startingPosition = transform.position;
            startingIndex = currentWaypointIndex;
        }
       
        bool horizontalNotFrozen = (rigidBodyComponent.constraints & RigidbodyConstraints2D.FreezeRotation) != RigidbodyConstraints2D.None
                                &&
                                (rigidBodyComponent.constraints & RigidbodyConstraints2D.FreezePositionY) != RigidbodyConstraints2D.None;

        if (horizontalNotFrozen)
        {
            horizontalWaypoints = true;
        }
      

        
    }


    [PunRPC]
    protected override void DoFunction(int p_targetObjectID)
    {
       


    }
    protected void OnTriggerEnter2D(Collider2D p_collider)
    {
        if (wayPointsList.Count > 0)
        {
            if (wayPointsList.Contains(p_collider.transform))
            {
                StartCoroutine(StayInPositionDelay());
            }
        }
    }
  
    public void MoveTowardsNextWaypoint()
    {
        


      


        if (currentWaypointIndex >= wayPointsList.Count - 1 || currentWaypointIndex <= 0) // If platform reaches the first or last waypoint, it returns back
        {
            isAscendingWaypointIndex = isAscendingWaypointIndex ? false : true;
      
        }

        if (isAscendingWaypointIndex)
        {
            currentWaypointIndex++;
           
        }
        else
        {
            currentWaypointIndex--;
          
        }

        currentSpeed = maxSpeed;
        constantForceComponent.force = (wayPointsList[currentWaypointIndex].position - transform.position).normalized * currentSpeed;

       
    }

    IEnumerator StayInPositionDelay()
    {
        currentSpeed = 0;
        constantForceComponent.force = new Vector2(0, 0);
        rigidBodyComponent.constraints = RigidbodyConstraints2D.FreezeAll;
        yield return new WaitForSeconds(1f);
        MoveTowardsNextWaypoint();
        if (horizontalWaypoints)
        {
            rigidBodyComponent.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        }
        else
        {
            rigidBodyComponent.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }
    }


}
