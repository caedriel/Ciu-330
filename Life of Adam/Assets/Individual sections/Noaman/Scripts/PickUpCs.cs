﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PickUpCs : MonoBehaviour
{
    [Header("Add player object here")]
    [SerializeField]
    private FpcontrollerCs fp;
    [Header("Add the place slot here")]
    [SerializeField]
    private GameObject redPlacePos;
    [SerializeField]
    private GameObject bluePlacePos;
    [SerializeField]
    private GameObject greenPlacePos;
    [Header("Add the child object 'PickPoint'")]
    [SerializeField]
    private GameObject pickupPoint;
    [SerializeField]
    private GameObject cam;
    [Header("Object you picked")]
    [SerializeField]
	private GameObject pickedObj;
    private GameObject tempPickedObj;
    [Header("Value to pickup object")]
    [SerializeField]
	private float Dist;
    [SerializeField]
    private LevelManagerCs day;
	private Rigidbody rb;

    [SerializeField]
    private AudioClip myClip;
    [SerializeField]
    private AudioSource myAudio;

	bool isholding;
	bool canDrop;
	// Use this for initialization
	void Start()
	{
		rb = GetComponent<Rigidbody>();
		fp = GetComponent<FpcontrollerCs>();
        isholding = false;

		if (GameObject.Find("LevelManager") != null)
		{
			day = GameObject.Find("LevelManager").GetComponent<LevelManagerCs>();
		}
		else
		{
			day = null;
		}
	}

	void Update()
	{

		PickUp();
	}

	void PickUp()
	{
		//add pickup code 
		Vector3 fwd = cam.transform.TransformDirection(Vector3.forward);
        RaycastHit hit;

		// when a object can be picked up 
        if (Physics.Raycast(cam.transform.position, fwd, out hit, Mathf.Infinity)&&!isholding)
		{

			if ((Input.GetMouseButtonDown(0)) && isholding == false && fp != null)
			{
				Debug.DrawRay(cam.transform.position, cam.transform.TransformDirection(Vector3.forward) * 10, Color.black);
				//Debug.Log("Did Hit");
				if (hit.collider.gameObject.tag == "Red" || hit.collider.gameObject.tag == "Blue"|| hit.collider.gameObject.tag == "Green" || hit.collider.gameObject.tag == "Pick")
				{
                    //Debug.Log("PICKUP CALLED");
                    // hinge joint is causing issues with the collision
                    tempPickedObj = hit.collider.gameObject;
                    fp.onAnim(6);
                    fp.setHolding(true);
                    // disbaled to test method call from animation
                    /* hit.collider.gameObject.transform.rotation = Quaternion.identity;
                     hit.collider.gameObject.GetComponent<Rigidbody>().useGravity = false;
                     pickedObj = hit.collider.gameObject;
                     //hit.collider.gameObject.GetComponent<Rigidbody>().constraints
                     hit.collider.gameObject.transform.position = pickupPoint.transform.position;
                     hit.collider.gameObject.transform.parent = pickupPoint.transform;
                     isholding = true;
                     canDrop = false;
                     //hit.collider.gameObject.AddComponent<FixedJoint>();
                     //hit.collider.gameObject.GetComponent<FixedJoint>().connectedBody =rb;
                     //fp.setSpeed(15f);
                     GetComponent<UiHandlerCs>().setRay(false);
                     //pickupPoint.GetComponentInChildren<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                     Debug.Log("Picked object");

                     fp.setHolding(true);*/
                }
			}

		}
		// when a object can be dropped 
		else if ((Input.GetMouseButtonDown(0)) && isholding == true &&Time.timeScale >= 1)
		{
            //Debug.Log("trying to drop object");
			//Debug.Log(Vector3.Distance(this.transform.position, bluePlacePos.transform.position));
			//hit.collider.gameObject.GetComponent<Test>().setSlotActive();
			if (pickedObj.tag == "Blue"&& Vector3.Distance (this.transform.position, bluePlacePos.transform.position)<Dist&& bluePlacePos!=null)
			{
                fp.onAnim(7);
				//Debug.Log("blue code called ");
                Rigidbody tempRb= GetComponentInChildren<Rigidbody>();
				//fp.setSpeed(4.0f);
				//Destroy(pickupPoint.GetComponentInChildren<FixedJoint>());
				tempRb.useGravity = true;
				//pickupPoint.transform.DetachChildren();
				tempRb.constraints  &= ~(RigidbodyConstraints.FreezePositionX |RigidbodyConstraints.FreezePositionY| RigidbodyConstraints.FreezePositionZ) ;
				pickedObj.SetActive(false);
				//pickedObj = null;
				//bluePlacePos.GetComponent<PuzzleCs>().setSlotActive();
				isholding = false;
				//day.setBlueTrue();
				GetComponent<UiHandlerCs>().setRay(true);
                pickupPoint.transform.DetachChildren();
                pickedObj = null;
                bluePlacePos.GetComponent<PuzzleCs>().setSlotActive();
                day.setBlueTrue();
                fp.setHolding(false);
                //fp.setHolding(false);
            }
			else if (pickedObj.tag == "Red" && Vector3.Distance (this.transform.position, redPlacePos.transform.position)<Dist&&redPlacePos!= null)
			{
                fp.onAnim(7);
                //Debug.Log("red code called ");
				Rigidbody tempRb = GetComponentInChildren<Rigidbody>();
				//fp.setSpeed(8.0f);
               // Destroy(pickupPoint.GetComponentInChildren<FixedJoint>());
				tempRb.useGravity = true;
				pickupPoint.transform.DetachChildren();
				tempRb.constraints  &= ~(RigidbodyConstraints.FreezePositionX |RigidbodyConstraints.FreezePositionY| RigidbodyConstraints.FreezePositionZ) ;
				pickedObj.SetActive(false);
				pickedObj = null;
			    //redPlacePos.GetComponent<PuzzleCs>().setSlotActive();
				isholding = false;
				day.setRedTrue();
				GetComponent<UiHandlerCs>().setRay(true);
                pickupPoint.transform.DetachChildren();
                pickedObj = null;
                redPlacePos.GetComponent<PuzzleCs>().setSlotActive();
                day.setRedTrue();
                fp.setHolding(false);
                //fp.setHolding(false);
            }
            else if (pickedObj.tag == "Green" && Vector3.Distance(this.transform.position, greenPlacePos.transform.position) < Dist && greenPlacePos != null)
            {
                fp.onAnim(7);
               // Debug.Log("Green code called ");
                Rigidbody tempRb = GetComponentInChildren<Rigidbody>();
                //fp.setSpeed(8.0f);
               // Destroy(pickupPoint.GetComponentInChildren<FixedJoint>());
                tempRb.useGravity = true;
                pickupPoint.transform.DetachChildren();
                tempRb.constraints &= ~(RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ);
                pickedObj.SetActive(false);
                pickedObj = null;
                //greenPlacePos.GetComponent<PuzzleCs>().setSlotActive();
                isholding = false;
                day.setGreenTrue();
                GetComponent<UiHandlerCs>().setRay(true);
                pickupPoint.transform.DetachChildren();
                pickedObj = null;
                greenPlacePos.GetComponent<PuzzleCs>().setSlotActive();
                day.setGreenTrue();
                fp.setHolding(false);
                //fp.setHolding(false);
            }
            else if (pickedObj != null &&isholding)
			{
                fp.onAnim(0);
                //Debug.Log("Null object called ");
				//fp.setSpeed(8.0f);
				pickupPoint.GetComponentInChildren<Rigidbody>().constraints &= ~(RigidbodyConstraints.FreezePositionX |RigidbodyConstraints.FreezePositionY| RigidbodyConstraints.FreezePositionZ) ;
                //Destroy(pickupPoint.GetComponentInChildren<FixedJoint>());
				//Debug.Log("Object dropped");
				pickupPoint.GetComponentInChildren<Collider>().enabled = true;
				pickupPoint.GetComponentInChildren<Rigidbody>().useGravity = true;
				pickupPoint.transform.DetachChildren();
				pickedObj = null;
				isholding = false;
				GetComponent<UiHandlerCs>().setRay(true);
                fp.setHolding(false);
            }


		}

		if (pickupPoint != null && isholding == true)
		{
			pickedObj.transform.position = pickupPoint.transform.position;
		}
	}
    void PushEvent()
    {
        if (pickedObj.tag == "Blue")
        {

        }
        else if (pickedObj.tag == "Red")
        {

        }
        else if (pickedObj.tag == "Green")
        {
           
        }
    }

    // called from the animator
    void PickEvent()
    {
        //Debug.Log("PICKUP EVENT CALLED");
        tempPickedObj.transform.rotation = Quaternion.identity;
        tempPickedObj.GetComponent<Rigidbody>().useGravity = false;
        pickedObj = tempPickedObj;
        //hit.collider.gameObject.GetComponent<Rigidbody>().constraints
        tempPickedObj.transform.position = pickupPoint.transform.position;
        tempPickedObj.transform.parent = pickupPoint.transform;
        isholding = true;
        canDrop = false;
        //hit.collider.gameObject.AddComponent<FixedJoint>();
        //hit.collider.gameObject.GetComponent<FixedJoint>().connectedBody =rb;
        //fp.setSpeed(15f);
        GetComponent<UiHandlerCs>().setRay(false);
        //pickupPoint.GetComponentInChildren<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        //Debug.Log("Picked object");


    }
    public void DropObjectOnDie()
    {
        if (pickedObj != null)
        {
            Debug.Log("Dropper");
            //Debug.Log("Null object called ");
            //fp.setSpeed(8.0f);
            pickupPoint.GetComponentInChildren<Rigidbody>().constraints &= ~(RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ);
            //Destroy(pickupPoint.GetComponentInChildren<FixedJoint>());
            //Debug.Log("Object dropped");
            pickupPoint.GetComponentInChildren<Collider>().enabled = true;
            pickupPoint.GetComponentInChildren<Rigidbody>().useGravity = true;
            pickupPoint.transform.DetachChildren();
            pickedObj = null;
            isholding = false;
            GetComponent<UiHandlerCs>().setRay(true);
            fp.setHolding(false);
        }
    }
    public void setCanDrop(bool drop)
	{

		canDrop = drop;

	}

	public bool isholdingCheck()
	{
		return isholding; 
	}
}