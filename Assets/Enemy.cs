using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class Enemy : Interactible
{
    public int currentWaypointIndex;
    public List<Transform> wayPointsList = new List<Transform>();
    public float currentSpeed;
    public float breakSpeed;
    public float maxSpeed;
    public bool isReturning;
    public bool isAscendingWaypointIndex;

    public ConstantForce2D constantForceComponent;
    public Rigidbody2D rigidBodyComponent;

    public float damage;


    private void OnValidate()
    {
        breakSpeed = Mathf.Min(0.1f, breakSpeed);//makes sure breakspeed is 0.1 so it can leave the trigger waypoint
    }
    public override void Start()
    {
        base.Start();
        damage = 1;
        currentWaypointIndex = 0;
        isReturning = false;
        isAscendingWaypointIndex = false;
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
        }


    }

    [PunRPC]
    protected override void DoFunction(int p_targetObjectID)
    {
        base.DoFunction(p_targetObjectID);

        if (PhotonView.Find(p_targetObjectID).gameObject.GetComponent<Health>())
        {
            PhotonView.Find(p_targetObjectID).gameObject.GetComponent<Health>().TakeDamage(damage);
        }
    }

    protected void OnTriggerEnter2D(Collider2D p_collider)
    {

        if (p_collider.gameObject.layer == LayerMask.NameToLayer("Trigger"))
        {
            if (wayPointsList.Count > 0)
            {
                if (isReturning == false)
                {
                    currentSpeed = breakSpeed;
                    constantForceComponent.force = (wayPointsList[currentWaypointIndex].position - transform.position).normalized * currentSpeed;
                }
            }
        }
    }

    protected void OnTriggerExit2D(Collider2D p_collider)
    {
        if (wayPointsList.Count > 0)
        {
            if (wayPointsList.Contains(p_collider.transform))
            {
                isReturning = isReturning ? false : true;
                currentSpeed = maxSpeed;
                if (isReturning == true)
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

                    constantForceComponent.force = (wayPointsList[currentWaypointIndex].position - transform.position).normalized * currentSpeed;
                    if (constantForceComponent.force.x > 0)
                    {
                        rigidBodyComponent.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                    }
                    else if (constantForceComponent.force.y > 0)
                    {
                        rigidBodyComponent.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                    }

                }

            }
        }
    }


}
