using UnityEngine;
using UnityEngine.AI;

public class NPCFollower : MonoBehaviour
{
    public Transform player;
    public float followDistance = 5f;

    private NavMeshAgent agent;
    private bool shouldFollow = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (shouldFollow && player != null)
        {
            agent.SetDestination(player.position);
        }
    }

    public void TryFollowPlayer()
    {
        if (player == null)
        {
            Debug.LogWarning($"{name}: Player not assigned.");
            return;
        }

        float dist = Vector3.Distance(transform.position, player.position);
        Debug.Log($"{name} clicked. Distance to player: {dist}");

        if (dist <= followDistance)
        {
            shouldFollow = true;
            Debug.Log($"{name} started following player.");
        }
        else
        {
            Debug.Log($"{name} is too far to follow.");
        }
    }
}
