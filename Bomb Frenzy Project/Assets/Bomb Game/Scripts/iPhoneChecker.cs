using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class iPhoneChecker : MonoBehaviour {
	public CanvasScaler _canvas;
	public bool iSiPhoneX;
	// Use this for initialization
	void Start () {
		//check if this is an iphoneX
		iSiPhoneX = ThisPlatform.IsIphoneX;

		//change the screen scale size
		if (iSiPhoneX) 
		_canvas.matchWidthOrHeight = 0.5f;
	}

	void Update()
	{
		if (iSiPhoneX) 
			_canvas.matchWidthOrHeight = 0.5f;
	}
}
