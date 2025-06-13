using UnityEngine;

public class BookCubeBehavior : MonoBehaviour, IInteractable
{
    [Header("References")]
    public DiseaseBookUI bookUI;
	public PlayerMovement playerMovement;

    private DiseaseDatabase diseaseDatabase;

    private bool isBookOpen = false;

    void Start()
    {
        diseaseDatabase = new DiseaseDatabase();

        if (bookUI == null)
            Debug.LogError("BookCubeBehavior: DiseaseBookUI reference is missing.");
    }

    // This determines where the player should go to interact (just stand in front of the cube)
    public Vector3 GetApproachPosition(Transform playerTransform, float stoppingDistance)
    {
		Vector3 directionToPlayer = (transform.position - playerTransform.position).normalized;
		// Offset position stoppingDistance units away from cube center towards player
		Vector3 approachPos = transform.position - directionToPlayer * stoppingDistance;
		return approachPos;
    }

    // When interacted with, open the book if not already open
    public void Interact()
    {
        if (!isBookOpen)
        {
			bookUI.OpenBook(diseaseDatabase, this);
			playerMovement.inputEnabled = false;
            isBookOpen = true;
            Debug.Log("Book opened.");
        }
    }

    public void NotifyBookClosed()
    {
        isBookOpen = false;
		playerMovement.inputEnabled = true;
    }
}
