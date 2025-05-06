[System.Serializable]
public class FurnitureItem
{
    public string Id;
    public string Name;
    public string Name__c;
    // Add more fields like below if you have them in Salesforce
    public string Category__c;
    public float Price__c;
    public string Image_URL__c;
}

[System.Serializable]
public class FurnitureWrapper
{
    public int totalSize;
    public bool done;
    public FurnitureItem[] records;
}
