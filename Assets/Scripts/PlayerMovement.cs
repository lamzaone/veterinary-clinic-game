using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
	private Camera cam;  // Reference to the main camera
    private NavMeshAgent agent;  // NavMeshAgent on the player

	[SerializeField] private float stopDistanceFromNPC = 5f;
	[SerializeField] private float rotationSpeed = 5f;

	private Vector3? lookTarget = null;
	private GameObject targetNPC = null;

    // Start is called before the first frame update
    void Start()
    {
		agent = GetComponent<NavMeshAgent>();
		if (cam == null)
			cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
		MoveOnMouseClick();
		SmoothRotate();

		// Automatically interact when close to target NPC
		if (targetNPC != null && !agent.pathPending && agent.remainingDistance <= stopDistanceFromNPC)
		{
			InteractWithNPC(targetNPC);
			targetNPC = null;
		}
    }

	void MoveOnMouseClick()
	{
        if (Input.GetMouseButtonDown(0)) { // Left click
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) {

				Transform target = hit.transform;

				if (target.CompareTag("Ground")) {
					agent.SetDestination(hit.point);
					lookTarget = hit.point;
					targetNPC = null; // Clear NPC target
				} else if (target.CompareTag("NPC")) {
					float distanceToNPC = Vector3.Distance(transform.position, target.position);

					if (distanceToNPC <= stopDistanceFromNPC) {
						// Already close — interact immediately
						InteractWithNPC(target.gameObject);
						targetNPC = null;
					}
					else {
						// Too far — move to NPC and interact on arrival
						Vector3 directionToNPC = (transform.position - target.position);
						Vector3 destination = target.position + directionToNPC * stopDistanceFromNPC;

						NavMeshHit navHit;
						if (NavMesh.SamplePosition(destination, out navHit, 1.0f, NavMesh.AllAreas)) {
							agent.SetDestination(navHit.position);
							lookTarget = target.position;
							targetNPC = target.gameObject; // Store for later interaction
						}
					}
				}
            }
        }        
	}
	void SmoothRotate()
	{
		if (lookTarget.HasValue) {
			Vector3 direction = (lookTarget.Value - transform.position).normalized;
			direction.y = 0; // Prevent tilting up/down

			if (direction.magnitude > 0.1f) {
				Quaternion targetRotation = Quaternion.LookRotation(direction);
				transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
			}
		}
	}

	void InteractWithNPC(GameObject npcObject)
	{
		NPC npcScript = npcObject.GetComponent<NPC>();
		if (npcScript != null)
		{
			npcScript.ShowSpeechBubble();
		}
		else
		{
			Debug.LogWarning("No NPC script found on: " + npcObject.name);
		}
	}
}
