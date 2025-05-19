using UnityEngine;

public class ClickHandler : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // left-click
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                NPCFollower follower = hit.transform.GetComponent<NPCFollower>();
                if (follower != null)
                {
                    follower.TryFollowPlayer();
                }
            }
        }
    }
}
