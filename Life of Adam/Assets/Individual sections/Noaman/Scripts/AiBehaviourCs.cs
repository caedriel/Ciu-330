﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

enum AiState
{
    Idle =0 , Chase =1 ,Patrol=2
};

public class AiBehaviourCs : MonoBehaviour
{
    [Header("Paste in the A object name here for enemy to follow path")]
    public string nameA;
    [Header("Paste in the B object name here for enemy to follow path")]
    public string nameB;
	public NavMeshAgent agent;
	public LayerMask layer;
    private AiState currentState;
    public Transform[] pathToFollowA;
    public Transform[] pathToFollowB;
    public int posPoint;
    [Header("MoveSpeed")]
    public float speed;
    [Header("ChaseSpeed")]
    public float chaseSpeed;
    public GameObject playerObj;
    [Header("TurnSpeed")]
	public float turnSpeed;
    [Header("MinDist before attacking player")]
	public float minDist;
    [Header("dist before ttack player")]
	public float attackDistance;
    [Header("RayCast dist before ttack player")]
    public float rayDistance;
    public LevelManagerCs day;
    
	private bool isPathA;
    private bool isPathB;

    private AiAnimator myAnimator;
    // Use this for initialization
    void Start ()
    {
        myAnimator = GetComponent<AiAnimator>();
        day = GameObject.FindObjectOfType<LevelManagerCs>();
        agent.autoBraking = false;
        if (!string.IsNullOrEmpty(nameA))
        {
            pathToFollowA = GameObject.Find(nameA).GetComponentsInChildren<Transform>();
        }
        if (!string.IsNullOrEmpty(nameB))
        {
            pathToFollowB = GameObject.Find(nameB).GetComponentsInChildren<Transform>();
        }

		agent.autoBraking = false;
		isPathA = true;
		isPathB = false;
        //myAnimator = GetComponent<AiAnimator>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (nameA!=null||nameB!=null)
        {
            //Debug.Log("AI WORKING");
            aiUpdate();
        }

    }


    void aiUpdate()
    {
        // stuff for collision avoidence 
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        //normalized vector between left & forward 
        Vector3 leftD = transform.TransformDirection(Vector3.left + Vector3.forward).normalized;
        //normalized vector between right & forward 
        Vector3 rightD = transform.TransformDirection(Vector3.right + Vector3.forward).normalized;

        RaycastHit hit;

        Debug.DrawLine(transform.position, transform.position + fwd.normalized * (rayDistance*2f), Color.red);
        Debug.DrawLine(transform.position, transform.position + leftD.normalized * (rayDistance *1.2f), Color.red);
        Debug.DrawLine(transform.position, transform.position + rightD.normalized * (rayDistance *1.2f), Color.red);



        if (Physics.Raycast(transform.position, fwd, out hit, attackDistance, layer, QueryTriggerInteraction.Ignore) ||
            Physics.Raycast(transform.position, leftD, out hit, attackDistance, layer, QueryTriggerInteraction.Ignore) ||
            Physics.Raycast(transform.position, rightD, out hit, attackDistance, layer, QueryTriggerInteraction.Ignore))
        {
            currentState = AiState.Chase;
            Debug.Log("chase");
          
        }

        if (Vector3.Distance(this.transform.position, playerObj.transform.position) <= 1f)
        {

            Debug.Log("You just died");
            playerObj.GetComponent<FpcontrollerCs>().OnDie();
            day.resetLevel();
            currentState = AiState.Patrol;
        }

        if (Vector3.Distance(this.transform.position, playerObj.transform.position) <= minDist)
        {
            currentState = AiState.Chase;
        }
        else
        {
            currentState = AiState.Patrol;
        }


        switch (currentState)
        {

            case AiState.Idle:
                idle();
                break;
            case AiState.Chase:
                // Seek();
                chase();
                break;
            case AiState.Patrol:
                //Flee();
                if (!agent.pathPending && agent.remainingDistance < 0.5f)
                {
                    if (isPathA)
                    {
                        patrol(pathToFollowA);
                    }
                    else if (isPathB)
                    {
                        patrol(pathToFollowB);
                    }
                }


                break;

        }
    }

	void chase()
	{
		agent.SetDestination(playerObj.transform.position);
        agent.speed = chaseSpeed;
        myAnimator.SetChase();
	}


	void patrol(Transform [] arr)
	{
        agent.speed = speed;
        if (arr.Length == 0)
            return;
        myAnimator.SetPatrol();
        // Set the agent to go to the currently selected destination.
        agent.destination = arr[posPoint].position;
        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        posPoint = (posPoint + 1) % arr.Length;

	}

	public void setA()
	{
		isPathA = true;
        isPathB = false;
	}
	public void setB()
	{
		isPathB = true;
        isPathA = false;
	}

	void idle()
	{
		Debug.Log("Is Idle");
	}

}