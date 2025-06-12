using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    private Camera cam;
    private NavMeshAgent agent;

    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float stoppingDistance = 1.6f;
    [SerializeField] private float interactionDistance = 3f;

    private Vector3? lookTarget = null;
    private IInteractable pendingInteractable = null;
    private Coroutine interactionCoroutine = null;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = stoppingDistance;
        cam = Camera.main;
    }

    void Update()
    {
        HandleClick();
        SmoothRotate();
    }

    void HandleClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("Ground"))
                {
                    // Cancel pending interaction and move to ground click point
                    pendingInteractable = null;
                    if (interactionCoroutine != null)
                    {
                        StopCoroutine(interactionCoroutine);
                        interactionCoroutine = null;
                    }
                    lookTarget = hit.point;
					agent.SetDestination(hit.point);
                }
                else
                {
                    // Try get interactable from clicked object or its parent
                    IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
                    if (interactable != null)
                    {
                        pendingInteractable = interactable;

						Vector3 approachPos = interactable.GetApproachPosition(transform, agent.stoppingDistance);
						lookTarget = approachPos;
						agent.SetDestination(approachPos);

                        if (interactionCoroutine != null)
                            StopCoroutine(interactionCoroutine);
                        interactionCoroutine = StartCoroutine(WaitUntilCloseThenInteract());
                    }
                }
            }
        }
    }

	IEnumerator WaitUntilCloseThenInteract()
	{
		while (pendingInteractable != null)
		{
			if (!agent.pathPending && agent.remainingDistance != Mathf.Infinity && agent.remainingDistance <= agent.stoppingDistance + 0.1f)
			{
				pendingInteractable.Interact();
				pendingInteractable = null;
				interactionCoroutine = null;
				yield break;
			}
			yield return null;
		}
	}

    void SmoothRotate()
    {
        if (lookTarget.HasValue)
        {
            Vector3 direction = (lookTarget.Value - transform.position).normalized;
            direction.y = 0;
            if (direction.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }
}

