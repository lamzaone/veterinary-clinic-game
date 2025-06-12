using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiseaseBookUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject bookPanel;

    // LEFT PAGE
    public TMP_Text headerTextLeft;
    public TMP_Text diseaseNameTextLeft;
    public TMP_Text descriptionTextLeft;
    public TMP_Text symptomsTextLeft;

    // RIGHT PAGE
    public TMP_Text headerTextRight;
    public TMP_Text diseaseNameTextRight;
    public TMP_Text descriptionTextRight;
    public TMP_Text symptomsTextRight;

    // NAVIGATION
    public Button nextButton;
    public Button previousButton;
    public Button closeButton;

    private List<(string animalType, Disease disease)> allDiseases = new();
    private int currentPage = 0;
    private BookCubeBehavior sourceCube;

    void Start()
    {
        bookPanel.SetActive(false);
        nextButton.onClick.AddListener(NextPage);
        previousButton.onClick.AddListener(PreviousPage);
        closeButton.onClick.AddListener(CloseBook);
    }

    public void OpenBook(DiseaseDatabase database, BookCubeBehavior caller = null)
    {
        sourceCube = caller;
        allDiseases.Clear();

        foreach (var kvp in database.diseasesByAnimal)
        {
            foreach (var disease in kvp.Value)
            {
                allDiseases.Add((kvp.Key, disease));
            }
        }

        currentPage = 0;
        UpdatePages();
        bookPanel.SetActive(true);
    }

    public void CloseBook()
    {
        bookPanel.SetActive(false);
        sourceCube?.CloseBook();
        sourceCube = null;
    }

    private void NextPage()
    {
        currentPage++;
        UpdatePages();
    }

    private void PreviousPage()
    {
        currentPage--;
        UpdatePages();
    }

    private void UpdatePages()
    {
        int leftIndex = currentPage * 2;
        int rightIndex = leftIndex + 1;

        if (leftIndex < allDiseases.Count)
            DisplayDisease(leftIndex, headerTextLeft, diseaseNameTextLeft, descriptionTextLeft, symptomsTextLeft);
        else
            ClearPage(headerTextLeft, diseaseNameTextLeft, descriptionTextLeft, symptomsTextLeft);

        if (rightIndex < allDiseases.Count)
            DisplayDisease(rightIndex, headerTextRight, diseaseNameTextRight, descriptionTextRight, symptomsTextRight);
        else
            ClearPage(headerTextRight, diseaseNameTextRight, descriptionTextRight, symptomsTextRight);

        previousButton.interactable = currentPage > 0;
        nextButton.interactable = (currentPage + 1) * 2 < allDiseases.Count;
    }

    private void DisplayDisease(int index, TMP_Text header, TMP_Text name, TMP_Text desc, TMP_Text symptoms)
    {
        var (animal, disease) = allDiseases[index];
        header.text = animal;
        name.text = disease.name;
        desc.text = disease.description;
        symptoms.text = string.Join(", ", disease.symptoms);
    }

    private void ClearPage(TMP_Text header, TMP_Text name, TMP_Text desc, TMP_Text symptoms)
    {
        header.text = name.text = desc.text = symptoms.text = "";
    }
}

