using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class NPCBehavior : MonoBehaviour, IInteractable 
{
    [Header("Positions")]
    public Vector3 spawnPoint;           // Where NPC starts and returns to despawn
    public Vector3 waitingRoomPoint;     // Target point inside waiting room

    [Header("Timing")]
    public float waitMinTime = 120f;     // 2 minutes
    public float waitMaxTime = 240f;     // 4 minutes

    private NavMeshAgent agent;
    private float waitTime;              // Random wait duration
    private float waitTimer = 0f;

    [HideInInspector] public bool caseSolved = false;

    private AnimalFollower animalFollower;
    private Transform playerTransform;

    private bool isSelected = false;
    private bool isLeaving = false;

    // Event to notify GameManager when NPC leaves
    public delegate void NPCLeftHandler(NPCBehavior npc, bool leftByTimeout);
    public event NPCLeftHandler OnNPCLeft;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null) {
            Debug.LogError("NPCBehavior requires a NavMeshAgent component.");
        }

    }

	void Start()
	{
		// Find the animal follower in children
		animalFollower = GetComponentInChildren<AnimalFollower>();
		// Find player
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		if (player != null) {
			playerTransform = player.transform;
		}
		//  Start by following the NPC (its master)
		if (animalFollower != null)
		{
			animalFollower.Follow(transform, 1.5f);
		}
	}

    // Call this to start the NPC walking in from spawn to waiting room
    public void StartWalkingIn()
    {
        isSelected = false;
        isLeaving = false;
        waitTimer = 0f;
        caseSolved = false;

        waitTime = Random.Range(waitMinTime, waitMaxTime);

        agent.updateRotation = true;
        agent.isStopped = false;
        agent.SetDestination(waitingRoomPoint);

        StartCoroutine(WaitForArrival());
    }

    IEnumerator WaitForArrival()
    {
        // Wait until NPC reaches waiting room
        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null;
        }

        // Arrived, stop agent and start waiting
        agent.isStopped = true;
        waitTimer = 0f;

        StartCoroutine(WaitForPlayer());
    }

    IEnumerator WaitForPlayer()
    {
        // Wait while not selected and not leaving
        while (!isSelected && !isLeaving)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTime)
            {
                // Timed out, NPC leaves
                Leave(true);
                yield break;
            }
            yield return null;
        }
    }

    // Try to select this NPC. Returns true if selection succeeds
    public bool TrySelect()
    {
        if (isSelected || isLeaving)
            return false;

        isSelected = true;
        agent.isStopped = true;  // Stop moving when selected
        return true;
    }

    // Called when deselecting and NPC needs to leave
    public void DeselectAndLeave()
    {
        if (!isSelected) return;

        isSelected = false;
		Deselect();
        Leave(false);
    }

    // NPC leaves by going back to spawn and then despawning
    public void Leave(bool leftByTimeout)
    {
        if (isLeaving) return;

        isLeaving = true;
        agent.isStopped = false;
        agent.updateRotation = true;
        agent.SetDestination(spawnPoint);

        StartCoroutine(WaitForDeparture(leftByTimeout));
    }

    IEnumerator WaitForDeparture(bool leftByTimeout)
    {
        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null;
        }

        // Notify GameManager NPC left
        OnNPCLeft?.Invoke(this, leftByTimeout);

        // Destroy the NPC GameObject (despawn)
        Destroy(gameObject);
    }

    public void Select()
    {
        if (animalFollower != null && playerTransform != null)
        {
            animalFollower.Follow(playerTransform, 1.5f);
            Debug.Log($"{name}'s animal is now following the player.");
        }
    }

    public void Deselect()
    {
        if (animalFollower != null)
        {
            animalFollower.Follow(transform, 1.5f); // follow master again
            Debug.Log($"{name}'s animal is returning to the NPC.");
        }
    }

	public Vector3 GetApproachPosition(Transform playerTransform, float stoppingDistance)
	{
		return transform.position;
	}

	public void Interact()
	{
		GameManager.Instance.TrySelectClient(this);
		Debug.Log($"{name} NPC selected.");
	}

}
