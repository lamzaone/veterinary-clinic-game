using UnityEngine;
using UnityEngine.AI;

public class AnimalFollower : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform currentTarget;
    private float followDistance = 1.5f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = followDistance;
    }

    void Update()
    {
        if (currentTarget != null)
        {
            agent.SetDestination(currentTarget.position);
        }
    }

    public void Follow(Transform newTarget, float stopDistance = 1.5f)
    {
        currentTarget = newTarget;
        if (agent != null)
        {
            agent.stoppingDistance = stopDistance;
        }
    }

    public void StopFollowing()
    {
        currentTarget = null;
        agent.ResetPath();
    }
}

