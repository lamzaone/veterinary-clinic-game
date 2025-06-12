using UnityEngine;
using System.Collections.Generic;

public class NPCSpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    public GameObject npcPrefab;
    public int numberToSpawn = 5;
    public float spawnInterval = 2f;

    [Header("Spawn Area")]
    public Vector3 spawnAreaCenter = new Vector3(80f, 0, 0);
    public Vector3 spawnAreaSize = new Vector3(20f, 0, 20f);
    public float minDistanceBetweenNPCs = 1.5f;

    [Header("Waiting Room")]
    public Vector3 waitingRoomCenter = new Vector3(27.5f, 0, 0);
    public Vector3 waitingRoomSize = new Vector3(10f, 0, 10f);
    public float minDistanceInWaitingRoom = 2f;

    private List<Vector3> spawnPositionsUsed = new List<Vector3>();
    private List<Vector3> waitingPositionsUsed = new List<Vector3>();
    private int spawnedCount = 0;
    private float timer = 0f;

    void Update()
    {
        if (spawnedCount >= numberToSpawn) return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnNPC();
            timer = 0f;
        }
    }

    void SpawnNPC()
    {
        Vector3 spawnPos = GetNonOverlappingPosition(spawnAreaCenter, spawnAreaSize, spawnPositionsUsed, minDistanceBetweenNPCs);
        Vector3 waitPos = GetNonOverlappingPosition(waitingRoomCenter, waitingRoomSize, waitingPositionsUsed, minDistanceInWaitingRoom);

        GameObject npcObj = Instantiate(npcPrefab, spawnPos, Quaternion.identity);
		AssignRandomColorToNPC(npcObj);

        NPCBehavior npcBehavior = npcObj.GetComponent<NPCBehavior>();
        if (npcBehavior != null)
        {
            npcBehavior.spawnPoint = spawnPos;
            npcBehavior.waitingRoomPoint = waitPos;

            npcBehavior.OnNPCLeft += OnNPCLeftHandler;

            npcBehavior.StartWalkingIn();

            spawnedCount++;
        }
        else
        {
            Debug.LogError("NPC prefab missing NPCBehavior script.");
        }
    }

    Vector3 GetNonOverlappingPosition(Vector3 areaCenter, Vector3 areaSize, List<Vector3> usedPositions, float minDistance)
    {
        const int maxAttempts = 20;

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 candidate = GetRandomPosition(areaCenter, areaSize);
            bool valid = true;

            foreach (var pos in usedPositions)
            {
                if (Vector3.Distance(candidate, pos) < minDistance)
                {
                    valid = false;
                    break;
                }
            }

            if (valid)
            {
                usedPositions.Add(candidate);
                return candidate;
            }
        }

        Debug.LogWarning("Could not find non-overlapping position. Returning random anyway.");
        return GetRandomPosition(areaCenter, areaSize);
    }

    Vector3 GetRandomPosition(Vector3 center, Vector3 size)
    {
        float x = Random.Range(-size.x / 2f, size.x / 2f);
        float z = Random.Range(-size.z / 2f, size.z / 2f);
        return new Vector3(center.x + x, center.y, center.z + z);
    }

    void OnNPCLeftHandler(NPCBehavior npc, bool leftByTimeout)
    {
        // Remove waiting position so it can be reused
        waitingPositionsUsed.Remove(npc.waitingRoomPoint);

        // Optionally handle score or game events here or forward to GameManager
        GameManager.Instance.OnNPCLeft(npc, leftByTimeout);

        spawnedCount--; // Allow spawning more NPCs if needed
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(spawnAreaCenter, spawnAreaSize);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(waitingRoomCenter, waitingRoomSize);
    }

	void AssignRandomColorToNPC(GameObject npc)
	{
		Color randomColor = new Color(Random.value, Random.value, Random.value);

		Renderer[] renderers = npc.GetComponentsInChildren<Renderer>();

		foreach (Renderer rend in renderers)
		{
			if (rend != null && rend.material != null)
			{
				// To avoid modifying shared material in editor, instantiate a new material instance
				rend.material = new Material(rend.material);
				rend.material.color = randomColor;
			}
		}
	}
}
