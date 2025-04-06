using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
	[SerializeField] private float moveSpeed = 6f; // Movement speed
	[SerializeField] private float moveAccel = 100f; // Movement acceleration

	// The player AI pathfinding
    private NavMeshAgent playerAI;
	// Camera for raycasting
	private Camera cam; 

    // Start is called before the first frame update
    void Start()
    {
		// Use Main Camera for camera
		cam = Camera.main;

		// Set playerAI to the NavMeshAgent component of the player
		playerAI = GetComponent<NavMeshAgent>();
		// Set speed for walking player
        playerAI.speed = moveSpeed;
		// Set acceleration for walking player
        playerAI.acceleration = moveAccel;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastMove();
    }

    void RaycastMove()
    {
		// If left click is pressed
        if (Input.GetMouseButtonDown(0)) {
			// Get the position on the ground where the player clicked
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

			// If ray hits some object with the "Ground" tag
			if (Physics.Raycast(ray, out hit) &&
					hit.collider.CompareTag("Ground")) {
                // Move the player to the clicked position
                playerAI.SetDestination(hit.point);
            }
        }
    }
}
