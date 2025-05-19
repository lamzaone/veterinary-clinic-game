using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateNpcSpeechBubble : MonoBehaviour
{
	private Camera cam;

	void Start()
	{
		cam = Camera.main;
	}

	void LateUpdate()
	{
		if (cam != null)
		{
			// Make the speech bubble face the camera
			transform.forward = cam.transform.forward;
			transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward,
					cam.transform.rotation * Vector3.up);
		}
	}
}
