using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
	[SerializeField] private GameObject speechBubble;  // Assign in prefab inspector

    void Start()
    {   
        if (speechBubble != null)
            speechBubble.SetActive(false);
    }

    public void ShowSpeechBubble()
    {
        if (speechBubble != null) {
            speechBubble.SetActive(true);
 			StartCoroutine(WaitAndHide());
		}
    }

	IEnumerator WaitAndHide()
    {
        yield return new WaitForSeconds(3f);  // Wait 3 seconds
        HideSpeechBubble();  // Call your function after wait
    }

    public void HideSpeechBubble()
    {
        if (speechBubble != null)
            speechBubble.SetActive(false);
    }
}
