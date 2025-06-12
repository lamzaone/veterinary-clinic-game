using UnityEngine;

public interface IInteractable
{
    /// <summary>
    /// Returns the position the player should approach for interaction.
    /// </summary>
    Vector3 GetApproachPosition(Transform playerTransform, float stoppingDistance);

    /// <summary>
    /// Trigger the interaction behavior (called when player is close enough).
    /// </summary>
    void Interact();
}

