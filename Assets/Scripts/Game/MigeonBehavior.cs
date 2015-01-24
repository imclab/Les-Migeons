﻿using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
public class MigeonBehavior : MonoBehaviour {

	private Transform player;

	protected int[] actionsList ;
	protected int maxActions ;
	protected int stepAction = 1 ;
	protected int repeatAction = 0 ;

	protected float moveDistance = 5.0f ;
	protected float speed = 1f ;
	protected float speedRotation = 0.5f ;
	protected bool jobToDo = true ;
	protected Vector3 target ;
	protected Vector3 eulerAngleTarget ;
	
	protected bool isGoingForward = false ;
	protected bool isTurning = false ;
	
	public bool carried { get; set; }

	// Use this for initialization
	void Start () {
		player = GameObject.Find("Player").transform;

		carried = false ;
		maxActions = Random.Range (5, 10);
		//maxActions = 8 ;
		actionsList = new int[maxActions+1];
		actionsList[0] = Random.Range (2, 5);
		//actionsList[0] = 3 ;
		for (int i = 1; i <= maxActions; i++) {
			actionsList[i] = Random.Range(0,5) ;
			//actionsList[i] = 1 ;
		}
		/*
		actionsList[1] = 0 ;
		actionsList[2] = 2 ;
		actionsList[3] = 0 ;
		actionsList[4] = 2 ;
		actionsList[5] = 0 ;
		actionsList[6] = 4 ;
		actionsList[7] = 1 ;
		actionsList[8] = 0 ;
		Debug.Log(maxActions + " " + actionsList[0]) ;
		Debug.Log(actionsList) ;
		*/
	}
	
	void Update(){
		if(carried){
			jobToDo = false ;
			isGoingForward = false ;
			isTurning = false ;
		}
	}

	// Update is called once per frame
	void FixedUpdate() {
		if(jobToDo){
			doYourJob() ;
		}

		if (carried)
		{
			rigidbody.Sleep();
			transform.position = player.position + transform.forward * 2f;
			transform.forward = Camera.main.transform.forward;
		}
		else
		{
			rigidbody.WakeUp();
		}
	}

	void startJob(){
		stepAction = 1 ;
		repeatAction = 0 ;
		jobToDo = true ;
	}

	void doYourJob(){
		switch(actionsList[stepAction]){
			case 0 :
				goForward() ;
			break;
			case 1 : case 2 :
				turn(actionsList [stepAction]) ;
				break;
			case 3 :
				//jump() ;
				Debug.Log("jump") ;
				break;
			case 4 :
				createBlock() ;
				stepAction++ ;
				break;
		}
		if(stepAction > maxActions){
			stepAction = 1 ;
			repeatAction++ ;
			Debug.Log("repeat") ;
			if(repeatAction >= actionsList[0]){
				Debug.Log("endJob") ;
				jobToDo = false ;
			}
		}
	}
	
	bool canIGoForward(Vector3 direction, float distance){
		if(Physics.Raycast(rigidbody.transform.position, direction, distance)){
			return false ;
		}
		return true ;
	}
	
	void goForward(){
		if(!isGoingForward){
			target = transform.forward*moveDistance + rigidbody.position ;
			target.x = Mathf.Round(target.x) ;
			target.y = Mathf.Round(target.y) ;
			target.z = Mathf.Round(target.z) ;
			Debug.Log("move "+target) ;
			isGoingForward = true ;
		}
		if(!canIGoForward(transform.forward, moveDistance+1.0f)){
			isGoingForward = false ;
			stepAction++ ;
			Debug.Log ("cant move, skip") ;
			return ;
		}
		Vector3 step = transform.forward*moveDistance*speed ;
		// Move our position a step closer to the target.
		rigidbody.MovePosition(rigidbody.transform.position + (step*Time.deltaTime));
		if(Vector3.Distance(rigidbody.transform.position, target) <= .5f){
			isGoingForward = false ;
			stepAction++ ;
		}
	}

	void turn(int direction){
		if(!isTurning){
			if (direction == 1) {
				//turn left
				eulerAngleTarget = Quaternion.Euler(rigidbody.rotation.eulerAngles + new Vector3(0f,-90f,0f)).eulerAngles ;
				Debug.Log("turn left "+eulerAngleTarget) ;
			}else{
				//turn right
				eulerAngleTarget = Quaternion.Euler(rigidbody.rotation.eulerAngles + new Vector3(0f,90f,0f)).eulerAngles ;
				Debug.Log("turn right "+eulerAngleTarget) ;
			}

			isTurning = true ;
			
		}
		
			float step = speedRotation * Time.deltaTime *100f ;
			Quaternion deltaRotation = Quaternion.Euler(eulerAngleTarget * step);
			//rigidbody.MoveRotation(rigidbody.rotation * deltaRotation) ;
			rigidbody.MoveRotation(Quaternion.RotateTowards(rigidbody.rotation, Quaternion.Euler(eulerAngleTarget), step)) ;
		if(Vector3.Distance(rigidbody.rotation.eulerAngles, eulerAngleTarget) <= 0.2f){
			rigidbody.MoveRotation(Quaternion.Euler(eulerAngleTarget)) ;
			isTurning = false ;
			stepAction++ ;
		}
	}

	void jump(){
		
	}

	void createBlock(){
		Debug.Log("create block") ;
		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cube.transform.position = transform.forward*2 + rigidbody.position ;
	}

}