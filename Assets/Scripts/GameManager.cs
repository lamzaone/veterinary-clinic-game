using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Gameplay")]
    public int maxClients = 1;  // You only allow one client at a time
    public int playerScore = 0;

    [HideInInspector]
    public NPCBehavior currentClient = null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

	public bool TrySelectClient(NPCBehavior npc)
	{
		if (npc == null) return false;

		if (currentClient == npc) return true;

		if (currentClient != null)
		{
			if (!currentClient.caseSolved)
			{
				playerScore -= 20; // Penalty for switching before solving
				Debug.Log("Abandoned client! -20 points. Score: " + playerScore);
			}

			currentClient.DeselectAndLeave();
		}

		if (npc.TrySelect())
		{
			currentClient = npc;
			currentClient.Select();
			Debug.Log("Selected NPC: " + npc.name); // Feedback here
			return true;
		}

		return false;
	}	

    public void SolveCurrentClient()
    {
        if (currentClient == null) return;

        currentClient.caseSolved = true;

        // Reward points for solving case
        playerScore += 100;

        // Let the NPC leave gracefully
        currentClient.DeselectAndLeave();

        currentClient = null;
    }

    public void OnNPCLeft(NPCBehavior npc, bool leftByTimeout)
    {
        if (npc == null) return;

        // If the NPC that left is the current client and case not solved => lose points
        if (currentClient == npc)
        {
            if (!npc.caseSolved)
            {
                LosePointsForClientLeaving();
            }

            currentClient = null;
        }

        // You can handle other cleanup or spawn more NPCs here if needed
    }

    private void LosePointsForSwitchingClient()
    {
        playerScore -= 50; // example penalty
        Debug.Log("Lost points for switching clients before solving case!");
    }

    private void LosePointsForClientLeaving()
    {
        playerScore -= 75; // example penalty
        Debug.Log("Lost points because client left without being helped!");
    }
}
