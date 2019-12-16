using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class HitPlane : MonoBehaviour
{

    public GameObject ProteinModels; // Big Models showing above head 
    public GameObject ShadowModels; 
    private GameObject BasicShapes;   // Small Models on Panel Canvas
    public Material CommonMat;

    private GameObject Combination; // for displaying the combined protein model
    private GameObject PrevCombina; // for checking if the Resource Load is successful

    string ActiveModelName; // current displaying Big Model's name

    public AudioClip WinClip;

    public bool isGoalAchieved; // Combination matched or Shape snapped

    // AssetBundle Variables
    AssetBundle myLoadedAssetBundle;
    string assetUrl = @"C:\Users\TEMP.CS-DOMAIN.001\Desktop\AssetBundles\AssetBundles\proteins";
    string assetName = "Complex";

    public Transform interactiveTargetPoint; // for dragging in the target points
    public Transform shadowModelTargetPoint;

    bool isHigherLevel; // for checking if it's level 1 or higher

    // AssetBundle Functions
    public void LoadAssetBundles(string url)
    {
        myLoadedAssetBundle = AssetBundle.LoadFromFile(url);
        Debug.Log(myLoadedAssetBundle == null ? "failed to load AssetBundle" : "AssetBundle successfully loaded");

        // know the name of each asset in this bundle
        string[] assetNames = myLoadedAssetBundle.GetAllAssetNames();
        int buttonIndex = 0; // will be modified later
        foreach (string name in assetNames)
        {
            string startPointToFind = "prefabs/";
            string finalPointToFind = ".prefab";
            int startIndex = name.IndexOf(startPointToFind);
            int finalIndex = name.IndexOf(finalPointToFind);
            int extractLength = finalIndex - startIndex - startPointToFind.Length;
            string proteinName = name.Substring(startIndex + startPointToFind.Length, extractLength);
            // assign proteinNames to buttons
            GameObject button = GameObject.Find("Asset Canvas").transform.Find("Test Asset" + buttonIndex).transform.Find("text").gameObject;
            button.GetComponent<TextMeshProUGUI>().text = proteinName;
            buttonIndex += 1;
        }
        
    }
    public void InstantiateAsset(string name)
    {
        var prefab = myLoadedAssetBundle.LoadAsset(name) as GameObject;
        Combination = Instantiate(prefab, ProteinModels.transform);
        Combination.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
        // Replace shaders with an Array (this depends on the object, Shape_IndexedTriangleStripSet might be needed)
        // GameObject ProteinMesh = Combination.transform.Find("Shape_IndexedTriangleStripSet").gameObject;
        GameObject ProteinMesh = Combination.gameObject;
        //Shape_IndexedTriangleStripSet
        Renderer r = ProteinMesh.GetComponent<Renderer>();
        Material[] m = new Material[r.materials.Length]; // names here are simplified
        for (int i = 0; i < m.Length; i++)
        {
            m[i] = CommonMat;
        }
        r.materials = m;
    }
    /* these two methods have been changed to public and called from OVRCanvas
    public void LoadAndShowCustomProtein()
    {
        LoadAssetBundles(assetUrl);
        InstantiateAsset(assetName);
    }
    */

    // Making Combination Functions
    void OnCollisionEnter (Collision col)
    {
        BasicShapes = GameObject.Find("Basic Shapes").gameObject; // reference basic shapes
        if (col.collider.transform.parent.name == BasicShapes.name) // if it's the Small Model hit on the Plane
        {
            // Main Purpose: Instantiate the combination

            GameObject ActiveMdoel = null; // store the previous active model
            ActiveModelName = ""; // reset the name each time

            // get the active model's name and hide it
            foreach (Transform child in ProteinModels.transform)
            {
                if (child.gameObject.activeInHierarchy)
                {
                    child.gameObject.SetActive(false);
                    ActiveModelName = child.name;
                    ActiveMdoel     = child.gameObject;
                }
            }

            Destroy(col.collider.gameObject); // destroy the flying object

            // two name possibilities
            string nameA = ActiveModelName + col.collider.name;
            string nameB = col.collider.name + ActiveModelName;

            // instantiate the combination
            Combination = Resources.Load("Combination/" + nameA) as GameObject; // try nameA
            if (Combination != null && Combination != PrevCombina)
            {
                Combination = Instantiate(Combination, ProteinModels.transform);
                Combination.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                // Replace shaders with an Array
                Renderer r = Combination.GetComponent<Renderer>();
                Material[] m = new Material[r.materials.Length]; // names here are simplified
                for (int i = 0; i < m.Length; i++)
                {
                    m[i] = CommonMat;
                }
                r.materials = m;
                // The model is not basic anymore
                // IsCombination = true;

                Combination.name = nameA;  // assigns GameObject's name
                PrevCombina = Combination; // storing Combination GameObject for comparing
                FindObjectOfType<AudioManager>().Play("Drop"); // play Drop sound effect
                FindObjectOfType<AudioManager>().Stop("Drag");
                GameObject.Find("Name Text").GetComponent<TextMeshProUGUI>().text = nameA;
            } // loaded successfully and Combination changed
            else
            {
                Combination = Resources.Load("Combination/" + nameB) as GameObject; // try nameB
                if (Combination != null && Combination != PrevCombina)
                {
                    Combination = Instantiate(Combination, ProteinModels.transform);
                    Combination.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                    // Replace shaders with an Array
                    Renderer r = Combination.GetComponent<Renderer>();
                    Material[] m = new Material[r.materials.Length]; // names here are simplified
                    for (int i = 0; i < m.Length; i++)
                    {
                        m[i] = CommonMat;
                    }
                    r.materials = m;
                    // The model is not basic anymore
                    // IsCombination = true;

                    Combination.name = nameB;
                    PrevCombina = Combination;
                    FindObjectOfType<AudioManager>().Play("Drop"); // play Drop sound effect
                    FindObjectOfType<AudioManager>().Stop("Drag");
                    GameObject.Find("Name Text").GetComponent<TextMeshProUGUI>().text = nameB;
                }
                else
                {
                    Debug.Log("The Combination " + nameA + " or " + nameB + " Doesn't Exist");
                    FindObjectOfType<AudioManager>().Stop("Drag");
                    if (ActiveMdoel != null) { ActiveMdoel.SetActive(true); } // new combination model not found, show the previous one
                    GameObject.Find("Label Canvas").GetComponent<Animator>().SetTrigger("ShowPrompt");
                    // show the prompt and hide it after 2 sec
                    // GameObject.Find("Bottom Prompt Panel").GetComponent<Animator>().SetBool("IsOnDisplay", true);
                    // StartCoroutine(HideBottomPromptAfter(2));
                    // StartCoroutine(ShowHighlightEffect());   // highlight the reset button 
                    return; // code below won't be executed
                } // loading failed
            }

            // check if the Goal is achieved
            if (Combination.name == "SheetHelix" && !isHigherLevel)
            {
                // once achieve the goal, stop rotation to clamp the shadow and model
                ProteinModels.GetComponent<Rotation>().speed = 0f;
                ProteinModels.transform.localRotation = Quaternion.Euler(0, 0, 0);
                Combination.transform.localRotation = Quaternion.Euler(0, 0, 0);
                // IsGoalAchieved = true;

                // Set the goal is achieved to true
                isGoalAchieved = true;
            } // the goal is achieved

            // check if the Goal of level 2 is achieved
            if (Combination.name == "SheetSheetfCoil")
            {
                // destroy the duplicated protein
                GameObject.Destroy(Combination);

                // once achieve the goal, stop rotation to clamp the shadow and model
                ProteinModels.GetComponent<Rotation>().speed = 0f;
                ProteinModels.transform.localRotation = Quaternion.Euler(0, 0, 0);
                ProteinModels.transform.Find("SheetSheetfCoil").gameObject.SetActive(true);
                // IsGoalAchieved = true;
            } // the goal is achieved
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool isHigherLevel = SceneManager.GetActiveScene().buildIndex > 0; // higher than level 1
        bool isLevel2 = SceneManager.GetActiveScene().buildIndex == 1; 
        bool isLevel3 = SceneManager.GetActiveScene().buildIndex == 2; 
        if (isHigherLevel && Vector3.Distance(interactiveTargetPoint.position, shadowModelTargetPoint.position) < 0.1f)
        {
            // Set the goal is achieved to true
            isGoalAchieved = true;
        }

        if (isGoalAchieved)
        {
            // Show Victory Effect
            FindObjectOfType<AudioManager>().Play("Victory");
            GameObject.Find("Game Canvas").GetComponent<Animator>().SetTrigger("Pop");
            VibrationManager.singleton.TriggerVibration(WinClip, OVRInput.Controller.LTouch);
            VibrationManager.singleton.TriggerVibration(WinClip, OVRInput.Controller.RTouch);
        }

        if (isGoalAchieved && isLevel2)
        {
            // loop through all bones,  snap there position with shadow bones'
            foreach (Transform child in ProteinModels.transform.Find("SheetSheetfCoil").GetComponentsInChildren<Transform>())
            {
                if (child.name.Contains("Bone"))
                {
                    foreach (Transform shadow_child in ShadowModels.transform.Find("SheetSheetfCoil").GetComponentsInChildren<Transform>())
                    {
                        if (shadow_child.name == child.name)
                        {
                            child.transform.rotation = shadow_child.rotation;
                        }
                    }
                }
            }
        }
        if (isGoalAchieved && isLevel3)
        {
            // loop through all bones,  snap there position with shadow bones'
            foreach (Transform child in ProteinModels.transform.Find("Complex").GetComponentsInChildren<Transform>())
            {
                if (child.name.Contains("Bone"))
                {
                    foreach (Transform shadow_child in ShadowModels.transform.Find("Complex").GetComponentsInChildren<Transform>())
                    {
                        if (shadow_child.name == child.name)
                        {
                            child.transform.rotation = shadow_child.rotation;
                        }
                    }
                }
            }
        }
    }
}
