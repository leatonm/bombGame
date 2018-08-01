using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soundControl : MonoBehaviour {

	public GameObject iconmute;
	public GameObject iconunmute;

	public void ToggleSound()
	{
		if (AudioListener.volume <= 0) {
			AudioListener.volume = 1f;
		} else {
			AudioListener.volume = 0f;
		}
	}

	void LateUpdate(){
		if (AudioListener.volume <= 0) {
			iconmute.SetActive (true);
			iconunmute.SetActive (false);
		} else {
			iconmute.SetActive (false);
			iconunmute.SetActive (true);
		}
	}


}