using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class onBoxHit : MonoBehaviour {

	public GameObject explosion;
	public static bool explode;
	int indexInt;
	public int PointsToGain;
	public GameObject FloatingText;
	public GameObject thisBox;
	private GameTimerController gt;




	void Start() {
		explode = false;
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player") {
			explodeAndPoint ();
			Destroy (other.gameObject);
		}
	}


	void explodeAndPoint()
	{
		if (!explode) {
			gt = GameObject.Find ("TimerController").GetComponent<GameTimerController> ();
			gt.sendHit (); //send the hit
			GameObject _Canvas = GameObject.Find ("Canvas");
			GameObject expl = Instantiate (explosion, transform.position, Quaternion.identity) as GameObject; //show explosion
			DamageManager.sharedInstance.DoDamage (PointsToGain, expl.transform.position); //gain the points
			GameObject go = Instantiate (FloatingText, _Canvas.transform.position, Quaternion.identity) as GameObject; //show the point text
			go.transform.parent = _Canvas.transform;
			thisBox.gameObject.SetActive (false); //hide this box
			explode = true; 
			Destroy (go, 0.5f); //remove the text
			Destroy (expl, 3);  //remove the explosion
		}
		TrytoSpawnSpecial ();
	}

	void TrytoSpawnSpecial ()
	{
		indexInt = Random.Range (1, 11);
		if (indexInt == 7) 
		{
			Spawner rspawner = GameObject.Find ("BoxSpawner").GetComponent<Spawner> ();
			rspawner.SpawnSpecial ();

			Debug.Log ("Spawn Lucky Block");

		} else
			return;
	}
		
}
