using UnityEngine;
using System.Collections.Generic;

public class NPCSpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    public GameObject npcPrefab;
    public int numberToSpawn = 5;
    public float spawnInterval = 2f;

    [Header("Spawn Area")]
    public Vector3 spawnAreaCenter = new Vector3(27.5f, 0, 0);
    public Vector3 spawnAreaSize = new Vector3(14f, 0, 19f);
    public float minDistanceBetweenNPCs = 1.5f;

    private List<Vector3> usedPositions = new List<Vector3>();
    private Transform player;
    private int spawnedCount = 0;
    private float timer = 0f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

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
        Vector3 spawnPos = GetNonOverlappingPosition();
        GameObject npc = Instantiate(npcPrefab, spawnPos, Quaternion.identity);

        NPCFollower follower = npc.GetComponentInChildren<NPCFollower>(true);
        if (follower != null && player != null)
        {
            follower.player = player;
        }

		// 🔽 NEW: Set the camera for the Canvas in the NPC
		Camera mainCam = Camera.main;
		Canvas canvas = npc.GetComponentInChildren<Canvas>(true); // 'true' includes inactive objects
		if (canvas != null && mainCam != null && canvas.renderMode != RenderMode.WorldSpace)
		{
			canvas.worldCamera = mainCam;
		}

		AssignRandomColorToChildren(npc);

        spawnedCount++;
    }

	void AssignRandomColorToChildren(GameObject npc)
	{
		// Generate a random color
		Color randomColor = new Color(Random.value, Random.value, Random.value);

		// Get all renderer components of all children
		Renderer[] renderers = npc.GetComponentsInChildren<Renderer>();

		// Set the material color for each renderer
		foreach (Renderer rend in renderers)
		{
			if (rend != null)
			{
				rend.material.color = randomColor;
			}
		}
	}

    Vector3 GetNonOverlappingPosition()
    {
        const int maxAttempts = 20;

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 candidate = GetRandomPosition();
            bool valid = true;

            foreach (var pos in usedPositions)
            {
                if (Vector3.Distance(candidate, pos) < minDistanceBetweenNPCs)
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

        // Fallback: allow overlap if needed
        Debug.LogWarning("Could not find non-overlapping position. Spawning anyway.");
        return GetRandomPosition();
    }

	Vector3 GetRandomPosition()
	{
		float x = Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f);
		float z = Random.Range(-spawnAreaSize.z / 2f, spawnAreaSize.z / 2f);
		float y = spawnAreaCenter.y; // or terrain sample

		return new Vector3(spawnAreaCenter.x + x, y, spawnAreaCenter.z + z);
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(spawnAreaCenter, spawnAreaSize);
	}
}
