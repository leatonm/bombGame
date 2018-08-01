using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]

public class ThrowRocket : MonoBehaviour {

	//where to spawn (a transform infront of camara or you can assign the camera
	float startTime, endTime, swipeDistance, swipeTime ;
	private Vector2 startPos;
	private Vector2 endPos;
	float tempTime;
	//the min distance to count it as a flick
	public float MinSwipDist = 0;
	private float FlickerLength;
	private float BallVelocity = 0;
	private float BallSpeed = 0;
	public float MaxBallSpeed = 40;
	private Vector3 angle;

	private bool thrown, holding;
	private Vector3 newPosition;


	public float smooth = 0.7f; 
	private Vector3 velocity;
	public GameObject specialFX;
	public AudioClip BombThrow;

	private GameTimerController gt;

	void OnTouch() {
		Vector3 mousePos = Input.GetTouch (0).position;
		mousePos.z = Camera.main.nearClipPlane * 9.5f;
		newPosition = Camera.main.ScreenToWorldPoint (mousePos);
		this.transform.localPosition = Vector3.Lerp (this.transform.localPosition, newPosition, 80f * Time.deltaTime);
	}

	void Start()
	{gt = GameObject.Find ("TimerController").GetComponent<GameTimerController> ();}

	void Update () {
		//====turn on special FX
		if (DamageManager.sharedInstance.getInstantSpawnStatus ()) {
			specialFX.SetActive (true);
		}else
		{
			specialFX.SetActive(false);
		}
		//=====FX=====


		if (holding) {
			OnTouch ();
		} else {}

		if (thrown)
			return;
		if(Input.touchCount > 0){
			Touch _touch = Input.GetTouch (0);

			if (_touch.phase == TouchPhase.Began) {

				Ray ray = Camera.main.ScreenPointToRay (Input.GetTouch (0).position); //for pc = Input.mousePosition
				RaycastHit hit;

				if (Physics.Raycast (ray, out hit, 100f)) {
					if (hit.transform == this.transform) {
						startTime = Time.time;
						startPos = _touch.position;
						holding = true;
						transform.SetParent (null);
					}
				}

			} else if (_touch.phase == TouchPhase.Ended && holding) {

				endTime = Time.time;
				endPos = _touch.position;
				swipeDistance = (endPos - startPos).magnitude;
				swipeTime = endTime - startTime;

				if (swipeTime < 0.5f && swipeDistance > 100f) {
					GetComponent<AudioSource> ().PlayOneShot (BombThrow);
					Speeds ();
					MoveAngle ();
					//Ball.GetComponent<Rigidbody> ().AddForce (new Vector3 ((angle.x * BallSpeed), (angle.y - BallSpeed), (angle.z * BallSpeed)));
					this.GetComponent<Rigidbody> ().AddForce (new Vector3 ((angle.x * BallSpeed), (angle.y * BallSpeed), (angle.z * BallSpeed)));
					this.GetComponent<Rigidbody> ().useGravity = true;
					//Debug.Log (angle.z);
					//play audio
					holding = false; //turn this off
					thrown = true;
					if (!DamageManager.sharedInstance.getInstantSpawnStatus()) {
						Invoke ("RespawnRocket", 0.4f);
						Destroy (this.gameObject, 5);
						gt.gainBombs (-1);
					} else {
						RespawnRocket();
						Destroy(this.gameObject, 8); 
					}
				} else {
					//Debug.Log ("Reset the ball");
					Reset ();
				}						
			}

			if (startTime > 0) 
			{
				//Debug.Log ("More than 0");
				tempTime = Time.time - startTime;
				if (tempTime > 0.5f) 
				{
					startTime = Time.time;
					startPos = _touch.position;
					//Debug.Log ("Too long");
				}
			}
		}
			
	}


	//speed setter
	void Speeds(){
		FlickerLength = swipeDistance;
		if (swipeTime > 0) {
			BallVelocity = FlickerLength / (FlickerLength - swipeTime);
		} else {
		}
		BallSpeed = BallVelocity * 80; //was 25
		BallSpeed = BallSpeed - (BallSpeed * 1.7f);
		if (BallSpeed <= -MaxBallSpeed) {

			BallSpeed = -MaxBallSpeed;
		}
		swipeTime = 0;
	}


	void MoveAngle(){
		angle = Camera.main.GetComponent<Camera> ().ScreenToWorldPoint (new Vector3 (endPos.x, endPos.y + 50f, (Camera.main.GetComponent<Camera> ().nearClipPlane - 9.0f)));
	}



	void Reset(){
		onBoxHit.explode = false;
		Transform SpawnPoint = GameObject.Find ("spawnPoint").transform;
		this.gameObject.transform.position = SpawnPoint.position;
		this.gameObject.transform.rotation = SpawnPoint.rotation;
		this.GetComponent<Rigidbody> ().velocity = Vector3.zero;
		this.GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;
		this.GetComponent<Rigidbody> ().useGravity = false;
		thrown = holding = false; //only this was in this method before
		this.gameObject.SetActive (true);
	}

	void RespawnRocket()
	{
		onBoxHit.explode = false;
		RocketSpawner rspawner = GameObject.Find ("spawnPoint").GetComponent<RocketSpawner> ();
		rspawner.spawnRocketNow ();
	}

}
