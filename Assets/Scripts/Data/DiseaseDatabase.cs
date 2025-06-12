using System.Collections.Generic;

public class DiseaseDatabase
{
    public Dictionary<string, List<Disease>> diseasesByAnimal = new Dictionary<string, List<Disease>>();

    public DiseaseDatabase()
    {
        // Example data for demo purposes
        diseasesByAnimal["Dog"] = new List<Disease>()
        {
            new Disease {
                name = "Canine Distemper",
                description = "A contagious viral disease affecting dogs.",
                symptoms = new List<string> { "Fever", "Nasal discharge", "Coughing", "Seizures" }
            },
            new Disease {
                name = "Parvovirus",
                description = "A highly contagious virus in dogs.",
                symptoms = new List<string> { "Vomiting", "Diarrhea", "Lethargy" }
            }
        };

        diseasesByAnimal["Cat"] = new List<Disease>()
        {
            new Disease {
                name = "Feline Leukemia",
                description = "Virus weakening cats' immune system.",
                symptoms = new List<string> { "Weight loss", "Poor appetite", "Fever" }
            }
        };
    }
}

