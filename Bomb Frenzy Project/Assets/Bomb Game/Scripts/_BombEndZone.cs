using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class _BombEndZone : MonoBehaviour {
	
	public GameTimerController gt;
	public GameObject LostBombPrefab;
	public GameObject CanvasParent;
	public Transform spawmpost;


	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "bad") {
			LostBombAnimation ("-20");
			gt.gainBombs(-20);
		} else if (other.gameObject.tag == "good") {
			//take away 1 bomb
			LostBombAnimation ("-5");
			gt.gainBombs(-5);
		}
	}

	void OnTriggerExit(Collider other)
	{
		Destroy (other.gameObject);
	}

	public void LostBombAnimation(string x)
	{
		GameObject go = Instantiate (LostBombPrefab, spawmpost.position, Quaternion.identity) as GameObject;
		go.transform.parent = CanvasParent.transform;
		Animator  animationObj = go.GetComponent<Animator>();
		go.gameObject.GetComponentInChildren<Text> ().text = x;
		animationObj.Play("LostBomb");
		Destroy (go, 0.7f);
//		Debug.Log ("play animation");
//		Animator  animationObj = LostBombPrefab.GetComponent<Animator>();
//		LostBombPrefab.gameObject.GetComponentInChildren<Text> ().text = x;
//		animationObj.Play("LostBomb");
	}
		
}
