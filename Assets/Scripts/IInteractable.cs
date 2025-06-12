using UnityEngine;

public interface IInteractable
{
    Vector3 GetApproachPosition(Transform playerTransform, float stoppingDistance);

    void Interact();
}
