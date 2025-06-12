using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    private Camera cam;
    private NavMeshAgent agent;

    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float stoppingDistance = 1f;
    [SerializeField] private float interactionDistance = 3f;

    private Vector3? lookTarget = null;
    private NPCBehavior pendingNPC = null;
    private Coroutine selectionCoroutine = null;

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
                if (hit.collider.CompareTag("NPC"))
                {
                    // Get the NPCBehavior from parent of collider (handles multi-child colliders)
                    NPCBehavior npc = hit.collider.GetComponentInParent<NPCBehavior>();
                    if (npc != null)
                    {
                        pendingNPC = npc;

                        // Calculate approach position (a bit away from NPC, so player doesn't overlap)
                        Vector3 directionFromNPC = (transform.position - npc.transform.position).normalized;
                        Vector3 approachPos = npc.transform.position + directionFromNPC * (stoppingDistance + 0.1f);

                        lookTarget = approachPos;
                        agent.SetDestination(approachPos);

                        // Start or restart coroutine to wait for close enough
                        if (selectionCoroutine != null)
                            StopCoroutine(selectionCoroutine);
                        selectionCoroutine = StartCoroutine(WaitUntilCloseThenSelect());
                    }
                }
                else if (hit.collider.CompareTag("Ground"))
                {
                    // Cancel any pending NPC selection on ground click
                    pendingNPC = null;
                    if (selectionCoroutine != null)
                    {
                        StopCoroutine(selectionCoroutine);
                        selectionCoroutine = null;
                    }

                    lookTarget = hit.point;
                    agent.SetDestination(hit.point);
                }
            }
        }
    }

    IEnumerator WaitUntilCloseThenSelect()
    {
        while (pendingNPC != null)
        {
            float distance = Vector3.Distance(transform.position, pendingNPC.transform.position);
            if (distance <= interactionDistance)
            {
                GameManager.Instance.TrySelectClient(pendingNPC);
                Debug.Log($"Selected NPC: {pendingNPC.name}");

                pendingNPC = null;
                selectionCoroutine = null;
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

