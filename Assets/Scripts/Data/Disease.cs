using System.Collections.Generic;

[System.Serializable]
public class Disease
{
    public string name;
    public string description;
    public List<string> symptoms = new List<string>();
}
