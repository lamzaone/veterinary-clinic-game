using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
	// Player to follow and rotate around
	[SerializeField] private Transform player; 
	[SerializeField] private float zoomSpeed = 25f;
	[SerializeField] private float zoomMin = 4f;
	[SerializeField] private float zoomMax = 12f;
	[SerializeField] private float zoomSmooth = 5f;
	[SerializeField] private float zoomCurrent = 8f;
	[SerializeField] private float rotationSpeed = 100f;
	[SerializeField] private float camRotationXMin = 0f;
	[SerializeField] private float camRotationXMax = 20f;

	// Camera with ortographic perspective
	private Camera cam; 

    // Start is called before the first frame update
	private void Start()
	{
		// Have IsoCameraParent take Player position
		transform.position = player.position;
		// Use Main Camera for camera
		cam = Camera.main;
	}

    // Update is called once per frame
	private void Update()
	{
		Rotate();
		Zoom();
		FollowPlayer();
	}

	private void Zoom()
	{
		// Keep zoom level inside limits
		zoomCurrent = Mathf.Clamp(zoomCurrent - Input.mouseScrollDelta.y *
				zoomSpeed * Time.deltaTime, zoomMin, zoomMax);	

		// Smooth zoom change by changing the size proprety of the ortographic
		// camera
		cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, zoomCurrent,
				zoomSmooth * Time.deltaTime);
	}

	private void Rotate()
	{
		// If right click is pressed
		if (Input.GetMouseButton(1)) {

			// Get the mouse movement on both axes
			float mouseX = Input.GetAxis("Mouse X");
			float mouseY = Input.GetAxis("Mouse Y");

			// Rotation around Y-axis (left-to-right)
			float rotationAroundY = mouseX * rotationSpeed * Time.deltaTime;
			// Rotation around X-axis (down-to-up)
			float rotationAroundX = -mouseY * rotationSpeed * Time.deltaTime;

			// Get current rotation and calculate the next rotation around the
			// X-axis
			Vector3 currentRotation = transform.rotation.eulerAngles;
			float potentialRotationX = currentRotation.x + rotationAroundX;

			// If there is more movement on the X-axis, rotate around Y-axis
			// (left-to-right)
			// 0.1 is mouse movement thresold might make things smoother for
			// diagonal rotation. maybe play with it a bit?
			if (Mathf.Abs(mouseX) > Mathf.Abs(mouseY) + 0.1f) {

				// Rotate only on the Y-axis (add to the rotation) and keep X
				// and Z unchanged
				currentRotation.y += rotationAroundY;
				// Apply the modified rotation back to the object
				transform.rotation = Quaternion.Euler(currentRotation);

				// If there is more mouse movement on the Y-axis, rotate around
				// X-axis and Z-axis (down-to-up)
			} else if (Mathf.Abs(mouseY) > Mathf.Abs(mouseX) + 0.1f) {

				// If we are still inside the limits, rotate 
				if (potentialRotationX >= camRotationXMin && 
						potentialRotationX <= camRotationXMax) {

					// Rotate both X and Z-axis, one in the opposite direction
					// to get the camera going "above"
					currentRotation.x += rotationAroundX;  
					currentRotation.z -= rotationAroundX; 

					transform.rotation = Quaternion.Euler(currentRotation);
				}
			}
		}
	}
	
	// Camera locked in on player
	private void FollowPlayer()
	{
		transform.position = player.position;
		transform.LookAt(player);
	}
}
