using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveboxForward : MonoBehaviour {
	private int speed;
	//public Material theMats;
	///private Material defaultMats;
	//private bool isMaterialChanged;




	// Update is called once per frame
	void Update () {
		transform.Translate(-Time.deltaTime * DamageManager.sharedInstance.getBlockSpeed(),0,0);

//		//change bomb texture if special is active
//		if(DamageManager.sharedInstance.getInstantSpawnStatus() && !isMaterialChanged)
//			{
//				//change box to good
//				gameObject.GetComponent<Renderer>().material = theMats;
//				isMaterialChanged = true;
//			}
//		else if (!DamageManager.sharedInstance.getInstantSpawnStatus() && isMaterialChanged){
//				//return to normal
//				isMaterialChanged = false;
//				gameObject.GetComponent<Renderer> ().material = defaultMats;
//			}
	}

	void Start()
	{
//		defaultMats = gameObject.GetComponent<Renderer> ().material;
		Destroy(this.gameObject, 60); // delete the explosion after 3 seconds
	}
}
