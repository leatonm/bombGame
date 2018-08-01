using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class explosionSound : MonoBehaviour {
	public AudioClip explosion;
	// Use this for initialization
	void Start () {
		GetComponent<AudioSource> ().PlayOneShot (explosion);
	}

}
