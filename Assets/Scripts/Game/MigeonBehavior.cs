﻿using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
public class MigeonBehavior : MonoBehaviour {

	private Transform player;

	protected int[] actionsList ;
	protected Genetics.GeneticCode code ;
	protected int maxActions ;
	protected int stepAction = 1 ;
	protected int repeatAction = 0 ;
	protected float distToFloor = 1.0f ;

	protected float moveDistance = 5.0f ;
	protected float speed = 1f ;
	protected float speedRotation = 0.5f ;
	protected bool jobToDo = true ;
	public Vector3 target ;
	public Vector3 targetJump ;
	public Vector3 eulerAngleTarget ;
	
	protected bool isGoingForward = false ;
	protected bool isTurning = false ;
	protected bool isJumping = false ;
	protected bool isFalling = false ;
	
	public bool carried { get; set; }
	protected bool wasCarried = false ;
	
	// Use this for initialization
	void Start () {
		code = Genetics.makeGeneticCode() ;
		//code.actions[] ;
		player = GameObject.Find("Player").transform;

		carried = false ;

		//maxActions = 5 ;
	}
	
	int[] getBaseActions(int idBase){
		int[] baseActions = new int[6] ;
		switch(idBase){
		case 1 :
			baseActions[0] = 10 ;
			baseActions[1] = 0 ;
			baseActions[2] = 4 ;
			baseActions[3] = 3 ;
			baseActions[4] = 4 ;
			baseActions[5] = 3 ;
		break ;
		
		case 2 :
			baseActions[0] = 10 ;
			baseActions[1] = 0 ;
			baseActions[2] = 4 ;
			baseActions[3] = 3 ;
			baseActions[4] = 0 ;
			baseActions[5] = 3 ;
		break ;
		
		case 3 :
			baseActions[0] = 10 ;
			baseActions[1] = 4 ;
			baseActions[2] = 2 ;
			baseActions[3] = 0 ;
			baseActions[4] = 4 ;
			baseActions[5] = 1 ;
		break;
		}
		return baseActions ;
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
		

		if (carried)
		{
			rigidbody.Sleep();
			transform.position = player.position + transform.forward * 2f;
			transform.forward = Camera.main.transform.forward;
			wasCarried = true ;
			
			isJumping = false ;
			isFalling = false ;
			isTurning = false ;
			isGoingForward = false ;
		}
		else if(wasCarried == true && carried == false)
		{
			snapToFloor() ;
			rigidbody.WakeUp();
			wasCarried = false ;
			startJob () ;
		}else if(jobToDo){
			doYourJob() ;
		}
	}

	void startJob(){
		stepAction = 0 ;
		repeatAction = 0 ;
		jobToDo = true ;
	}

	void doYourJob(){
		
		switch(code.actions[stepAction]){
			case Genetics.MIGEON_ACTION.AVANCER :
				goForward() ;
			break;
			case Genetics.MIGEON_ACTION.TURN_LEFT :
				turn (1) ;
				break ;
			case Genetics.MIGEON_ACTION.TURN_RIGHT :
				turn(2) ;
				break;
			case Genetics.MIGEON_ACTION.JUMP :
				jump() ;
				break;
			case Genetics.MIGEON_ACTION.PUT_CUBE :
				createBlock() ;
				break;
		}
		if(stepAction >= code.actions.Length-1){
			stepAction = 0 ;
			repeatAction++ ;
			if(repeatAction >= code.nbRepeat){
				jobToDo = false ;
			}
		}
	}
	
	bool canIGo(Vector3 direction, float distance){
		if(Physics.Raycast(rigidbody.transform.position+direction, direction, distance)){
			RaycastHit hit;
			Physics.Raycast(rigidbody.transform.position+direction, direction, out hit, distance) ;
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
		target.y = rigidbody.transform.position.y ;
		Vector3 dir = Vector3.Normalize(target-rigidbody.position) ;
		if(!canIGo(dir, moveDistance+0.1f)){
			isGoingForward = false ;
			stepAction++ ;
			Debug.Log ("cant move, skip") ;
			return ;
		}else{
			rigidbody.AddForce(dir*2f,ForceMode.Impulse) ;
		}
		
		RaycastHit hit;
		Physics.Raycast(rigidbody.transform.position, -Vector3.up, out hit) ;
		/*TODO GENERALIZE*/
		if(!isFalling && hit.distance >= 1f){
			isFalling = true ;
		}else if(isFalling && rigidbody.velocity.y < 0.1f && hit.distance <= 1f){
			snapToFloor() ;
			isFalling = false ;
		}else if(isFalling){
			rigidbody.AddForce(-Vector3.up*2f,ForceMode.Impulse) ;
		}

		if(Vector3.Distance(rigidbody.transform.position, target) <= .2f){
			rigidbody.MovePosition(target) ;
			snapToFloor() ;
			isFalling = false ;
			isGoingForward = false ;
			rigidbody.velocity = new Vector3(0f,0f,0f) ;
			stepAction++ ;
		}
	}

	void turn(int direction){
		if(!isTurning){
			if (direction == 1) {
				//turn left
				eulerAngleTarget = Quaternion.Euler(rigidbody.rotation.eulerAngles + new Vector3(0f,-90f,0f)).eulerAngles ;
				//Debug.Log("turn left "+eulerAngleTarget) ;
			}else{
				//turn right
				eulerAngleTarget = Quaternion.Euler(rigidbody.rotation.eulerAngles + new Vector3(0f,90f,0f)).eulerAngles ;
				//Debug.Log("turn right "+eulerAngleTarget) ;
			}

			isTurning = true ;
		}
		
		float step = speedRotation * Time.deltaTime *100f ;
		Quaternion deltaRotation = Quaternion.Euler(eulerAngleTarget * step);

		rigidbody.MoveRotation(Quaternion.RotateTowards(rigidbody.rotation, Quaternion.Euler(eulerAngleTarget), step)) ;
		if(Vector3.Distance(rigidbody.rotation.eulerAngles, eulerAngleTarget) <= 0.2f){
			rigidbody.MoveRotation(Quaternion.Euler(eulerAngleTarget)) ;
			isTurning = false ;
			stepAction++ ;
		}
	}

	void jump(){
		if(canIGo(Vector3.Normalize(transform.forward+transform.up),1.1f)){
			Debug.DrawLine(rigidbody.transform.position,rigidbody.transform.position + (transform.forward*1f + transform.up));
			if(!isJumping){
				targetJump = rigidbody.transform.position + (transform.forward*1.0f + transform.up) ;
				isJumping = true ;;
				rigidbody.AddForce((transform.up)*110f,ForceMode.Impulse) ;
			}
		}
		
		if(isJumping){		
			if(targetJump.y - rigidbody.transform.position.y >= 1.0f){
				rigidbody.AddForce((transform.forward)*0.5f,ForceMode.Impulse) ;
			}else if(targetJump.y - rigidbody.transform.position.y >= -1.0f){
				rigidbody.AddForce((transform.forward)*1f,ForceMode.Impulse) ;
				isFalling = true ;
			}
		}
		RaycastHit hit;
		Physics.Raycast(rigidbody.transform.position, -Vector3.up, out hit) ;

		if(isFalling && rigidbody.velocity.y <= 0.1f && hit.distance <= 1f){
			rigidbody.MovePosition(new Vector3(targetJump.x, rigidbody.transform.position.y, targetJump.z)) ;
			snapToFloor() ;
			isJumping = false ;
			isFalling = false ;
			rigidbody.velocity = new Vector3(0f,0f,0f) ;
			stepAction++ ;
		}else if(isFalling){
			rigidbody.AddForce(-Vector3.up*1.5f,ForceMode.Impulse) ;
		}
	}

	void createBlock(){
		float distance = 1 ;
		if(canIGo(rigidbody.transform.forward, distance+0.1f)){
			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			Vector3 newPos = rigidbody.position + transform.forward*distance ;
			newPos.x = Mathf.Round(newPos.x) ;
			newPos.y = Mathf.Round(newPos.y)-distToFloor/2f ;
			newPos.z = Mathf.Round(newPos.z) ;
			cube.transform.position = newPos ;
		}
		stepAction++ ;
	}
	
	void snapToFloor(){
		// Save current object layer
		int oldLayer = gameObject.layer;
		
		//Change object layer to a layer it will be alone
		gameObject.layer = LayerMask.NameToLayer("Ghost");
		
		int layerToIgnore = 1 << gameObject.layer;
		layerToIgnore = ~layerToIgnore;

		RaycastHit hit;
		if (Physics.Raycast(transform.position, -Vector3.up, out hit,0.5f,layerToIgnore)){
			if(Vector3.Distance(transform.position,hit.point+new Vector3(0.0f,distToFloor,0.0f))>=0.4f){
				Debug.Log ("snap :"+hit.collider.gameObject.name) ;
				transform.position = new Vector3(Mathf.Round(hit.point.x), Mathf.Round(hit.point.y), Mathf.Round(hit.point.z))+new Vector3(0.0f,distToFloor,0.0f) ;
			}
			float newY = Mathf.Round(transform.rotation.eulerAngles.y / 45.0f) * 45.0f ;
			//Debug.Log (transform.rotation.eulerAngles.y+" "+newY) ;
			transform.rotation = Quaternion.Euler(0.0f,newY,0.0f) ;
			//transform.position = transform.position ;
		}
		// set the game object back to its original layer
		gameObject.layer = oldLayer;
	}
	
}