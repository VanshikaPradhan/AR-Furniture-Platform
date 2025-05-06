using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class ProjectCardManager : MonoBehaviour
{
    public static ProjectCardManager Instance;

    public GameObject projectCardPrefab;
    public Transform cardContainer;
    public GameObject tagPrefab;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void CreateProjectCard(string imagePath)
    {
        Debug.Log("Creating card with imagePath: " + imagePath);
        if (!File.Exists(imagePath))
        {
            Debug.LogError("❌ Image file not found: " + imagePath);
            return;
        }

        byte[] bytes = File.ReadAllBytes(imagePath);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(bytes);

        GameObject card = Instantiate(projectCardPrefab, cardContainer);
        if (card == null)
        {
            Debug.LogError("Failed to instantiate project card prefab.");
            return;
        }//set title
        Transform titleTransform = card.transform.Find("ProjectTitle");
        if (titleTransform != null)
        {
            TextMeshProUGUI titleText = titleTransform.GetComponent<TextMeshProUGUI>();
            if (titleText != null)
            {
                titleText.text = "New Project";
            }
            else
            {
                Debug.LogError("TextMeshProUGUI not found on ProjectTitle.");
            }
        }
        else
        {
            Debug.LogError("ProjectTitle not found in prefab.");
        }
        //set image
        Transform imageTransform = card.transform.Find("ProjectImage");
        if (imageTransform != null)
        {
            Image img = imageTransform.GetComponent<Image>();
            if (img != null)
            {
                Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                img.sprite = sprite;
            }
            else
            {
                Debug.LogError("Image component not found on ProjectImage.");
            }
        }
        else
        {
            Debug.LogError("ProjectImage not found in prefab.");
        }

        //add dummy tags
        string[] tags = { "furniture", "wall decor" };
        Transform tagParent = card.transform.Find("Tags");
        if (tagParent != null)
        {
            foreach (string tag in tags)
            {
                GameObject tagObj = Instantiate(tagPrefab, tagParent);
                Text tagText = tagObj.GetComponentInChildren<Text>();
                if (tagText != null)
                {
                    tagText.text = tag;
                }
                else
                {
                    Debug.LogError("Text component not found in tag prefab.");
                }
            }
        }
        else
        {
            Debug.LogError("Tags container not found in prefab.");
        }

        //// Set progress
        //Slider slider = card.transform.Find("ProgressBar").GetComponent<Slider>();
        //slider.value = Random.Range(40f, 95f);




        Debug.Log("Project card created successfully.");

    }
}
