using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
	// Player to follow and rotate around
	[SerializeField] private Transform player; 
	// Camera with ortographic perspective
	private Camera cam; 

	[Header("Zoom Settings")]
	[SerializeField] private float zoomSpeed = 25f;
	[SerializeField] private float zoomMin = 5f;
	[SerializeField] private float zoomMax = 12f;
	[SerializeField] private float zoomSmooth = 5f;
	[SerializeField] private float zoomCurrent = 7f;

	[Header("Rotation Settings")]
	[SerializeField] private float rotationAmountY = 90f;
	[SerializeField] private float rotationAmountX = 10f;
	[SerializeField] private float inputThreshold = 0.7f;
	[SerializeField] private float rotationDuration = 0.4f;
	[SerializeField] private float bounceOvershoot = 10f;

	// Function to interpolate
	[SerializeField] private AnimationCurve bounceCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
	private bool isRotating = false;
	// Timer for function; use rotationDuration if you want to make animation faster/slower
	private float rotationTimer = 0f;

	private Quaternion startRotation;
	private Quaternion endRotation;
	private Quaternion overshootRotation;

	// Target rotations
    private float currentTargetY;
    private float currentTargetX;

    // Start is called before the first frame update
	private void Start()
	{
		// Have IsoCameraParent take Player position
		transform.position = player.position;
		// Use Main Camera for camera
		cam = Camera.main;

        currentTargetY = Mathf.Round(transform.eulerAngles.y / 90f) * 90f;
        currentTargetX = Mathf.Round(transform.eulerAngles.x / 90f) * 90f;
	}

    // Update is called once per frame
	private void Update()
	{
		Rotate();
		Zoom();
	}

	private void LateUpdate()
	{
		FollowPlayer();
	}

	private void Rotate()
	{
		// Get mouse axis
        if (!isRotating && Input.GetMouseButton(1)) { // Right-click held 
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = -Input.GetAxis("Mouse Y");

			// Rotate on the axis with the dominant movement and only if the threshold is reached.
            if ((Mathf.Abs(mouseX) - Mathf.Abs(mouseY)) > 0.05f) {
				if (mouseX > inputThreshold)
					StartRotation(1, 0);
				else if (mouseX < -inputThreshold)
					StartRotation(-1, 0);
			} else {
				if (mouseY > inputThreshold)
					StartRotation(1, 1);
				else if (mouseY < -inputThreshold)
					StartRotation(-1, 1);
			}
        }

		// Animate the rotation
        if (isRotating)
            AnimateRotation();	
	}

	// Rotate
    private void StartRotation(int direction, int axis)
    {
        isRotating = true;
        rotationTimer = 0f;
        startRotation = transform.rotation;

		// Rotate around Y axis
		if (axis == 0) {
			currentTargetY += rotationAmountY * direction;

			// Use transform.eulerAngles to keep the rotation on the other axis, that we do not
			// rotate around.
			endRotation = Quaternion.Euler(transform.eulerAngles.x, currentTargetY,
					transform.eulerAngles.z);
			overshootRotation = Quaternion.Euler(transform.eulerAngles.x, currentTargetY +
					bounceOvershoot * -direction, transform.eulerAngles.z);
		// Rotate around X & Z axis
		} else {
			currentTargetX += rotationAmountX * direction;

			// Clamp between 0 (start) and 20 degrees.
			currentTargetX = Mathf.Clamp(currentTargetX, 0, 20f);

			// Rotate around x and z in opposite directions to have that rising and lowering effect
			// As above, transform.eulerAngles to keep the rotation on the other axis, that we do
			// not rotate around.
			endRotation = Quaternion.Euler(currentTargetX, transform.eulerAngles.y,
					-currentTargetX);
			overshootRotation = Quaternion.Euler(currentTargetX + bounceOvershoot * -direction,
					transform.eulerAngles.y, (currentTargetX + bounceOvershoot * -direction) * -1);
		}
    }

	// Smooth animation for rotation
    private void AnimateRotation()
    {
		// Time the animation
        rotationTimer += Time.deltaTime;
        float t = rotationTimer / rotationDuration;

		// If rotation finished, exit.
        if (t >= 1f) {
            transform.rotation = endRotation;
            isRotating = false;
            return;
        }

        // Bounce curve controls the wobbliness
        float bounceT = bounceCurve.Evaluate(t);

        // Interpolate with bounce overshoot and then return to target
        Quaternion firstLerp = Quaternion.Slerp(startRotation, overshootRotation, bounceT);
        Quaternion finalLerp = Quaternion.Slerp(firstLerp, endRotation, bounceT);
        transform.rotation = finalLerp;
    }

	private void Zoom()
	{
		// Keep zoom level inside limits
		zoomCurrent = Mathf.Clamp(zoomCurrent - Input.mouseScrollDelta.y * zoomSpeed *
				Time.deltaTime, zoomMin, zoomMax);	

		// Smooth zoom change by changing the size proprety of the ortographic camera
		cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, zoomCurrent, 
				zoomSmooth * Time.deltaTime);
	}

	// Camera locked in on player
	private void FollowPlayer()
	{
		transform.position = Vector3.Lerp(transform.position, player.position, 5.0f * Time.deltaTime);
		transform.LookAt(transform);
	}
}
