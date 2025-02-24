﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class FpcontrollerCs : MonoBehaviour
{
    [Header("Pass in the camera here")]
	public GameObject camera;
	public float straffeVel;
    [Header("Speed of which player runs & walks")]
	public float runVel ;
	[SerializeField]
	private float walkVel;
	[Header("How high the player jumps")]
	public float jumpVel = 0.2f;
    [Header("Speed of which dies and returns to the start postion")]
	public float deathSmoothness;
    [Header("Speed of which player crouches 0.01=fast/0.3=smooth")]
	public float coruchSmoothness;
	private float forwardVel;
	private Vector3 velocity;
	private Vector3 IVeloctiy;
    [Header("Transform for camera to move to while not crocuhing")]
    [SerializeField]
	private Transform initialCrouch;
    [Header("Transform for camera to move to while crocuhing")]
    [SerializeField]
	private Transform crouchVect;
	private CapsuleCollider myCollider;
    [Header("When playe is crocuhed")]
    [SerializeField]
    private Vector3 crouchPosSize;
    [Header("When playe is not crocuhed")]
    [SerializeField]
    private Vector3 startCrouchSize;
	[Header("Players fall time before they die")]
	[SerializeField]
    float tempTime = 5f;
	[Header("refrence to the stamina image component")]
	public Image staminaUi;
	private float staminaUifill;
	Rigidbody rb;
	float forwardInput, straffeInput;
	[Header("stamina regen speed & stamina value (Do not change)")]
    [SerializeField]
    float staminaRegenSpeed;
    [SerializeField]
	float stamina;
    [Header("Place tun normal in 0 , run fast 1 , walk crouch at 3, 4 push walk  ")]
    [SerializeField]
    AudioClip []myClip;
    [Header(" either assign in unity or it will be assigned at runtime ")]
    [SerializeField]
    private Animator myAnim;

    bool isPlaying;
	bool canJump;
	bool canDie;

	bool canCrouch;
	bool isGrounded;

	//if true do action 
	bool canPush;
	bool canMove;

    bool canRun ;
    bool isRunning;
    bool isHolding;
    AudioSource aud;
    public LevelManagerCs level;

	Vector3 coruchVelocity = Vector3.zero;
	Vector3 startPos;
	Quaternion startRot;
    Vector3 lastPos;
    void Awake()
    {
        level = GameObject.Find("LevelManager").GetComponent<LevelManagerCs>();
    }
    void Start()
	{

        if (!myAnim)
        {
            myAnim = GetComponent<Animator>();
        }
        if (level.isHappy&&!staminaUi)
        {
            staminaUi = GameObject.FindGameObjectWithTag("Stamina").GetComponent<Image>();
        }
		startPos = transform.position;
		startRot = camera.transform.rotation;
		rb = GetComponent<Rigidbody>();
		forwardInput = straffeInput = 0;
		isPlaying = false;
		canCrouch = true;
		forwardVel = walkVel;
		myCollider = GetComponent<CapsuleCollider>();
		canMove = true;
		canPush = false;
		stamina = 100;
		staminaUifill = stamina / 100;
		canRun = true;
        aud = GetComponent<AudioSource>();
        isHolding = false;
	}

	void Update()
	{
        Vector3 currentPos = transform.position;
        bool tempColliderCheck = myCollider.gameObject.active;
        //AnimatorStateInfo stateInfo = myAnim.GetCurrentAnimatorStateInfo(0);
        // normal walk
        if (!isRunning&&!isHolding&& canCrouch && Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            //Debug.Log("OnWalk");
            //walk anim
            if (!isAnimPlaying("Walk 1"))
            {
                onAnim(1);
            }

        }
        // Idle
        // checks if not input is given via getaxis
        if (!isRunning&&canCrouch&&!isHolding &&currentPos ==lastPos)
        {
           //Debug.Log(stateInfo.fullPathHash);
            //Debug.Log("IDLE");
            onAnim(0);
        }
        //crouch walk 
        // checks if crouch has been triggered by !canCrouch and checks for input in any keys 
        if (!canCrouch && Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            onAnim(4);
        }
        // while is crouching and not moving go back to crouch idle
        else if (!canCrouch)
        {
            onAnim(3);
        }
        //hold animations cant be syned in time for showcase 
        if(isHolding&& Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            // play walk holding anim
            onAnim(5);
        }
        if (currentPos==lastPos && !canCrouch && !canRun && isHolding)
        {
            // play idle holding anim
            onAnim(8);
        }


        if (Time.timeScale >= 1)
		{
			Run();
			if (canMove)
			{
				if (!canPush)
				{
					forwardInput = Input.GetAxis("Vertical") * forwardVel;
					straffeInput = Input.GetAxis("Horizontal") * forwardVel;
				}
				else if (canPush && Input.GetAxis("Vertical") < 0 || Input.GetAxis("Vertical") > 0)
				{
					//vector3.back
					forwardInput = Input.GetAxis("Vertical") * forwardVel;
				}
			}
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) &&(!canCrouch&& !canPush))
            {
                if (!aud.isPlaying)
                {
                    aud.clip = myClip[0];
                    aud.Play();
                }
            }
            else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)&&canPush)
            {
                if (!aud.isPlaying)
                {
                    aud.clip = myClip[3];
                    aud.Play();
                }
            }
            else 
            {
                if (aud.isPlaying)
                {
                    aud.Pause();
                }
            }
			velocity = new Vector3(straffeInput, 0, forwardInput);
			velocity = transform.TransformDirection(velocity);
			rb.velocity = velocity;
			if (canJump & Input.GetKeyUp(KeyCode.Space))
			{
				rb.AddForce(new Vector3(0, jumpVel, 0), ForceMode.Impulse);
				canJump = false;
			}
			Crouch();
            GroundCheck();
            lastPos = currentPos;


            if (this.transform.position == startPos)
            {
                //Debug.Log("capsule reset called");

                this.GetComponent<CapsuleCollider>().enabled = true;
            }
        }
        
	}
	/* ground check is used to make sure the player has a platform under it , if its not we can make the player fall*/
	void GroundCheck()
	{
		RaycastHit hit;
		float distance = 3f;
		Vector3 dirD = Vector3.down;
		Vector3 pos = transform.position;
        //pos.x += 0.5f;
        bool canPlayerDie;
		if (Physics.Raycast(pos, dirD, out hit, distance))
		{
			rb.constraints= RigidbodyConstraints.FreezePositionY|RigidbodyConstraints.FreezeRotation;
            //Debug.Log(Physics.Raycast(pos, dirD, out hit, distance));
            canDie = true;
        
        }
		else
		{
            //Debug.Log(tempTime);
			myCollider.enabled = false;
			//Debug.Log(Physics.Raycast(pos, dirD, out hit, distance));
			rb.constraints = ~RigidbodyConstraints.FreezePositionY;
			rb.AddForce(new Vector3(0, -jumpVel, 0), ForceMode.Impulse);
			tempTime -= Time.deltaTime;
			if (tempTime<=0)
			{
                if (canDie)
                {
                    GetComponent<PickUpCs>().DropObjectOnDie();
                    // set bool back to true
                    Debug.Log("Die manyTIMES");
                    level.resetLevel();
                    canDie = false;
                }
                OnDie();
                tempTime = 5.0f;
			}
		}
	}
   /* Called when player crouched , initiated the coroutine to move back and forth to the crouch postion smoothly */
   /* we also adjust the colldier size so adam can go throguh small areas */
	void Crouch()
	{
		// on true
        if (canCrouch&& Input.GetKeyDown(KeyCode.LeftControl))
		{
            onAnim(3);
            canMove = false;
			//Vector3 mypos = myCollider.bounds.center;
			myCollider.center = crouchPosSize;
			myCollider.height = 0.93f;
			canCrouch = false;
            StopAllCoroutines();
			StartCoroutine(onCrouch(camera.transform, crouchVect.transform.position, coruchSmoothness));
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
            {
                if (!aud.isPlaying)
                {
                    aud.clip = myClip[2];
                    aud.Play();
                }
            }
            //Debug.Log("Crouch");


		}
		//on false 
		else if (!canCrouch && Input.GetKeyDown(KeyCode.LeftControl))
		{
			canMove = false;
			myCollider.center = startCrouchSize;
			myCollider.height = 1.7f;
			canCrouch = true;
            StopAllCoroutines();
			StartCoroutine(onCrouch(camera.transform, initialCrouch.transform.position, coruchSmoothness));
            onAnim(0);
        }

		
	}
	public void setSpeed(float mySpeed)
	{
		forwardVel = mySpeed;
	}
    /* code for player to run*/
	void Run()
	{
        if (Input.GetKey(KeyCode.LeftShift) && stamina >= 1&&(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)))
		{
            isRunning = true;
            onAnim(2);
            if (!aud.isPlaying)
            {
                aud.clip = myClip[1];
                aud.Play();
            }
            if (!canCrouch && !canPush)
            {
                // run anim

            }
            forwardVel = runVel;
			stamina--;
			staminaUifill = stamina / 100;
			staminaUi.fillAmount = staminaUifill;
			canRun = true;
		}
		else
		{
            isRunning = false;
            if (aud.isPlaying)
            {
                aud.Pause();
            }
			canRun = false;
		}
		/* stamin regen code */
		if (stamina <= 100.0f && canRun == false)
		{
			//Debug.Log("Stamina going up " + stamina);
			forwardVel = walkVel;
			stamina += Time.deltaTime* staminaRegenSpeed;
            staminaUifill = stamina /100;
			staminaUi.fillAmount = staminaUifill;

		}
	}
    //audio helper fucntion to check if a animation is playing 
    bool isAnimPlaying(string stateName)
    {
        if (myAnim.GetCurrentAnimatorStateInfo(0).IsName(stateName) &&
            myAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            return true;
        else
            return false;
    }
    public bool isPlayerHolding()
    {
        return isHolding;
    }
    public void setPush(bool push)
	{
		canPush = push;
	}
	void OnCollisionStay(Collision other)
	{
		canJump = true;
	}
    /* Called when player dies , initiated the coroutine to move back to the start postion smoothly */
	public void OnDie()
	{
        GetComponent<PickUpCs>().DropObjectOnDie();
        StopAllCoroutines();
		StartCoroutine(Die(this.transform, startPos, deathSmoothness));
	}

    public void setHolding(bool hold)
    {
        isHolding = hold;
    }
    public void onAnim(int an)
    {
        myAnim.SetInteger("PlayerMove", an);
    }

	private IEnumerator onCrouch(Transform transform, Vector3 position, float timeToMove)
	{
		Vector3 currentPos = camera.transform.position;
		float t = 0f;
		while (t < 1)
		{
			t += Time.deltaTime / timeToMove;
			transform.position = Vector3.Lerp(currentPos, position, t);

			if (camera.transform.position == position)
			{

				yield return null;
				//Debug.Log("canmove thingie working ");
				canMove = true;
			}

			yield return null;
		}
	}

	private IEnumerator Die(Transform transform, Vector3 position, float timeToMove)
	{
		myCollider.enabled.Equals(false);
		rb.constraints = RigidbodyConstraints.FreezeAll;
		canMove = false;
		Vector3 currentPos = this.transform.position;
		float t = 0f;
		while (t < 1)
		{
			t += Time.deltaTime / timeToMove;
			transform.position = Vector3.Lerp(currentPos, position, t);
			canMove = false;
			//rb.velocity = Vector3.zero;

			if (this.transform.position == startPos)
			{
				canMove = true;
				myCollider.enabled.Equals(true);
				rb.constraints = RigidbodyConstraints.None;
				//Debug.Log("lerping");
				rb.constraints= RigidbodyConstraints.FreezePositionY|RigidbodyConstraints.FreezeRotation;


				yield return null;
			}
            myCollider.enabled.Equals(true);
			yield return null;
		}

	}
}