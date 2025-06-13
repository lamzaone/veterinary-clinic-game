using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class DiseaseBookUI : MonoBehaviour
{
    private Canvas canvas;

    // Left page UI elements
    private TextMeshProUGUI diseaseNameTextLeft;
    private TextMeshProUGUI descriptionTextLeft;
    private RectTransform symptomsContainerLeft;

    // Right page UI elements
    private TextMeshProUGUI diseaseNameTextRight;
    private TextMeshProUGUI descriptionTextRight;
    private RectTransform symptomsContainerRight;

    // Buttons
    private Button prevButton;
    private Button nextButton;
    private Button closeButton;

    // Data
    private List<(string animal, Disease disease)> allDiseases;
    private int currentPageIndex = 0; // increments by 2 (2 diseases per page)

    private BookCubeBehavior caller;

    public void OpenBook(DiseaseDatabase database, BookCubeBehavior callingObject)
    {
        caller = callingObject;
        if (canvas != null) return; // Prevent duplicate UI

        allDiseases = new List<(string animal, Disease disease)>();

        foreach (var pair in database.diseasesByAnimal)
            foreach (var d in pair.Value)
                allDiseases.Add((pair.Key, d));

        if (allDiseases.Count == 0)
        {
            Debug.LogWarning("No diseases to display.");
            return;
        }

        // Create Canvas
        GameObject canvasGO = new GameObject("DiseaseBookCanvas");
        canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasGO.AddComponent<GraphicRaycaster>();

		// Create a full screen UI blocker panel to block clicks behind UI
		GameObject blockerPanel = new GameObject("UIBlockerPanel");
		blockerPanel.transform.SetParent(canvasGO.transform, false);

		RectTransform blockerRect = blockerPanel.AddComponent<RectTransform>();
		blockerRect.anchorMin = Vector2.zero;    // bottom-left
		blockerRect.anchorMax = Vector2.one;     // top-right
		blockerRect.offsetMin = Vector2.zero;    // no offset
		blockerRect.offsetMax = Vector2.zero;    // no offset

		UnityEngine.UI.Image blockerImage = blockerPanel.AddComponent<UnityEngine.UI.Image>();
		blockerImage.color = new Color(0, 0, 0, 0);  // fully transparent
		blockerImage.raycastTarget = true;            // very important — this makes it block clicks

        // Left page background
        GameObject leftPageGO = new GameObject("LeftPage");
        leftPageGO.transform.SetParent(canvasGO.transform, false);
        var leftPage = leftPageGO.AddComponent<RectTransform>();
        var leftImage = leftPageGO.AddComponent<UnityEngine.UI.Image>();
        leftImage.color = Color.white;

        leftPage.anchorMin = new Vector2(0f, 0f);
        leftPage.anchorMax = new Vector2(0.5f, 1f);
        leftPage.pivot = new Vector2(0f, 1f);
        leftPage.anchoredPosition = Vector2.zero;
        leftPage.sizeDelta = Vector2.zero;

        // Right page background
        GameObject rightPageGO = new GameObject("RightPage");
        rightPageGO.transform.SetParent(canvasGO.transform, false);
        var rightPage = rightPageGO.AddComponent<RectTransform>();
        var rightImage = rightPageGO.AddComponent<UnityEngine.UI.Image>();
        rightImage.color = Color.white;

        rightPage.anchorMin = new Vector2(0.5f, 0f);
        rightPage.anchorMax = new Vector2(1f, 1f);
        rightPage.pivot = new Vector2(0f, 1f);
        rightPage.anchoredPosition = Vector2.zero;
        rightPage.sizeDelta = Vector2.zero;

        // Left page UI elements (NO header)
        diseaseNameTextLeft = CreateTMP(leftPage, "DiseaseNameLeft", 18, TextAlignmentOptions.Center, new Vector2(0.5f, 1f), new Vector2(600, 50), new Vector2(0, -60));
        descriptionTextLeft = CreateTMP(leftPage, "DescriptionLeft", 14, TextAlignmentOptions.Top, new Vector2(0.5f, 1f), new Vector2(600, 110), new Vector2(0, -120));

        // CreateTMP(leftPage, "SymptomsLabelLeft", 14, TextAlignmentOptions.Left, new Vector2(0.5f, 1f), new Vector2(600, 25), new Vector2(0, -240)).text = "Symptoms:";
        GameObject symptomsLeftGO = new GameObject("SymptomsContainerLeft");
        symptomsLeftGO.transform.SetParent(leftPage, false);
        symptomsContainerLeft = symptomsLeftGO.AddComponent<RectTransform>();
        symptomsContainerLeft.anchorMin = new Vector2(0.5f, 1f);
        symptomsContainerLeft.anchorMax = new Vector2(0.5f, 1f);
        symptomsContainerLeft.pivot = new Vector2(0.5f, 1f);
		symptomsContainerLeft.anchoredPosition = new Vector2(50, -150);  // X increased (to right), Y less negative (higher)
		symptomsContainerLeft.sizeDelta = new Vector2(520, 180);        // Slightly narrower to keep padding consistent

		var layoutLeft = symptomsLeftGO.AddComponent<VerticalLayoutGroup>();
		layoutLeft.padding = new RectOffset(10, 10, 0, 0);
		layoutLeft.childAlignment = TextAnchor.UpperLeft;
		layoutLeft.spacing = 5;
		symptomsLeftGO.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // Right page UI elements (NO header)
        diseaseNameTextRight = CreateTMP(rightPage, "DiseaseNameRight", 18, TextAlignmentOptions.Center, new Vector2(0.5f, 1f), new Vector2(600, 50), new Vector2(0, -60));
        descriptionTextRight = CreateTMP(rightPage, "DescriptionRight", 14, TextAlignmentOptions.Top, new Vector2(0.5f, 1f), new Vector2(600, 110), new Vector2(0, -120));

        // CreateTMP(rightPage, "SymptomsLabelRight", 14, TextAlignmentOptions.Left, new Vector2(0.5f, 1f), new Vector2(600, 25), new Vector2(0, -240)).text = "Symptoms:";
        GameObject symptomsRightGO = new GameObject("SymptomsContainerRight");
        symptomsRightGO.transform.SetParent(rightPage, false);
        symptomsContainerRight = symptomsRightGO.AddComponent<RectTransform>();
        symptomsContainerRight.anchorMin = new Vector2(0.5f, 1f);
        symptomsContainerRight.anchorMax = new Vector2(0.5f, 1f);
        symptomsContainerRight.pivot = new Vector2(0.5f, 1f);
		symptomsContainerRight.anchoredPosition = new Vector2(50, -150);
		symptomsContainerRight.sizeDelta = new Vector2(520, 180);
		var layoutRight = symptomsRightGO.AddComponent<VerticalLayoutGroup>();
		layoutRight.padding = new RectOffset(10, 10, 0, 0);
		layoutRight.childAlignment = TextAnchor.UpperLeft;
		layoutRight.spacing = 5;
		symptomsRightGO.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;


        // Buttons container
        GameObject buttonsContainer = new GameObject("ButtonsContainer");
        buttonsContainer.transform.SetParent(canvasGO.transform, false);
        RectTransform buttonsRect = buttonsContainer.AddComponent<RectTransform>();
        buttonsRect.anchorMin = new Vector2(0f, 0f);
        buttonsRect.anchorMax = new Vector2(1f, 0f);
        buttonsRect.pivot = new Vector2(0.5f, 0f);
        buttonsRect.sizeDelta = new Vector2(0, 60);
        buttonsRect.anchoredPosition = new Vector2(0, 10);

        HorizontalLayoutGroup hLayout = buttonsContainer.AddComponent<HorizontalLayoutGroup>();
        hLayout.childAlignment = TextAnchor.MiddleCenter;
        hLayout.spacing = 40;
        hLayout.padding = new RectOffset(20, 20, 0, 0);

        // Prev Button
        prevButton = CreateButton(buttonsContainer.transform, "PrevButton", "Prev");
        prevButton.onClick.AddListener(OnPrevClicked);

        // Close Button
        closeButton = CreateButton(buttonsContainer.transform, "CloseButton", "Close");
        closeButton.onClick.AddListener(CloseBook);

        // Next Button
        nextButton = CreateButton(buttonsContainer.transform, "NextButton", "Next");
        nextButton.onClick.AddListener(OnNextClicked);

        UpdatePages();
        UpdateButtons();
    }

    private void UpdatePages()
    {
        ClearSymptoms(symptomsContainerLeft);
        ClearSymptoms(symptomsContainerRight);

        // Left page disease
        if (currentPageIndex < allDiseases.Count)
        {
            var (animal, disease) = allDiseases[currentPageIndex];
            diseaseNameTextLeft.text = disease.name;
            descriptionTextLeft.text = disease.description;
			foreach (var symptom in disease.symptoms)
			{
				var tmp = CreateTMP(symptomsContainerLeft, "Symptom", 14, TextAlignmentOptions.Center, Vector2.zero, Vector2.zero, Vector2.zero);
				tmp.text = $"- {symptom}";
				tmp.alignment = TextAlignmentOptions.Left;
				tmp.margin = new Vector4(50, 0, 0, 0);
			}
        }
        else
        {
            diseaseNameTextLeft.text = "";
            descriptionTextLeft.text = "";
        }

        // Right page disease
        int rightIndex = currentPageIndex + 1;
        if (rightIndex < allDiseases.Count)
        {
            var (animal, disease) = allDiseases[rightIndex];
            diseaseNameTextRight.text = disease.name;
            descriptionTextRight.text = disease.description;
			foreach (var symptom in disease.symptoms)
			{
				var tmp = CreateTMP(symptomsContainerRight, "Symptom", 14, TextAlignmentOptions.Center, Vector2.zero, Vector2.zero, Vector2.zero);
				tmp.text = $"- {symptom}";
				tmp.alignment = TextAlignmentOptions.Left;
				tmp.margin = new Vector4(50, 0, 0, 0);
			}
        }
        else
        {
            diseaseNameTextRight.text = "";
            descriptionTextRight.text = "";
        }
    }

    private void UpdateButtons()
    {
        prevButton.interactable = currentPageIndex > 0;

        int lastPageIndex = allDiseases.Count % 2 == 0 ? allDiseases.Count - 2 : allDiseases.Count - 1;
        nextButton.interactable = currentPageIndex < lastPageIndex;
    }

    private void OnPrevClicked()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex -= 2;
            if (currentPageIndex < 0) currentPageIndex = 0;
            UpdatePages();
            UpdateButtons();
        }
    }

    private void OnNextClicked()
    {
        int lastPageIndex = allDiseases.Count % 2 == 0 ? allDiseases.Count - 2 : allDiseases.Count - 1;
        if (currentPageIndex < lastPageIndex)
        {
            currentPageIndex += 2;
            UpdatePages();
            UpdateButtons();
        }
    }

    public void CloseBook()
    {
        if (canvas != null)
        {
            Destroy(canvas.gameObject);
            canvas = null;
        }
        caller?.NotifyBookClosed();
    }

    private void ClearSymptoms(RectTransform container)
    {
        foreach (Transform child in container)
            Destroy(child.gameObject);
    }

    private TextMeshProUGUI CreateTMP(Transform parent, string name, int fontSize, TextAlignmentOptions alignment,
            Vector2 anchor, Vector2 size, Vector2 offset)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = anchor;
        rect.anchoredPosition = offset;
        rect.sizeDelta = size;

        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.fontSize = fontSize;
        tmp.alignment = alignment;
        tmp.color = Color.black;
        tmp.enableWordWrapping = true;
        return tmp;
    }

    private Button CreateButton(Transform parent, string name, string buttonText)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        RectTransform rect = go.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(160, 40);

        var image = go.AddComponent<UnityEngine.UI.Image>();
        image.color = new Color(0.8f, 0.8f, 0.8f);

        var button = go.AddComponent<Button>();

        // Text child for button label
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(go.transform, false);
        RectTransform textRect = textGO.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0, 0);
        textRect.anchorMax = new Vector2(1, 1);
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        var tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = buttonText;
        tmp.fontSize = 18;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.black;

        return button;
    }
}

