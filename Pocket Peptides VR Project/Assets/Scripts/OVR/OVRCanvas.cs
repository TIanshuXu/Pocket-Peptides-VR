using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class OVRCanvas : MonoBehaviour
{
    public GameObject OVRPlayer;
    public GameObject BasicShapes; // reference Basic Shapes prefab for instantiation
    Transform target;

    bool isRotationToggled;

    public GameObject Sheet; // store prefabs for instantiation
    public GameObject Helix;
    public GameObject rCoil;
    public GameObject fCoil;

    HitPlane hp;

    // Start is called before the first frame update
    void Start()
    {
        target = OVRPlayer.transform;
        hp = FindObjectOfType<HitPlane>();
    }

    // Update is called once per frame
    void Update()
    {
        var lookPos = target.position - transform.position;
        lookPos.y = 0; // lock y axis
        var rotation = Quaternion.LookRotation(-lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 20f);
        

        /* same
        transform.LookAt(target);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + 180f, 0);
        */

        // if goal is achieved, show next level button
        if (FindObjectOfType<HitPlane>().isGoalAchieved)
        {
            GameObject.Find("Label Canvas").transform.Find("Formula").gameObject.SetActive(false);
            GameObject.Find("Label Canvas").transform.Find("NextLevel Button").gameObject.SetActive(true);
        }
    }

    public void ToggleRotation()
    {
        if (!isRotationToggled)
        {
            GameObject.Find("Protein Models").GetComponent<Rotation>().speed = 0;
            isRotationToggled = true;
        } else
        {
            GameObject.Find("Protein Models").GetComponent<Rotation>().speed = 25;
            isRotationToggled = false;
        }
    }

    public void Reset()
    {
        GameObject instance;

        foreach (Transform child in GameObject.Find("Protein Models").transform)
        {
            if (child.gameObject.activeInHierarchy)
            {
                child.gameObject.SetActive(false);
            }
        }
        // Display Sheet as a Basic Shape 
        // GameObject.Find("Protein Models").transform.Find("Sheet").gameObject.SetActive(true);

        Destroy(GameObject.Find("Basic Shapes").gameObject);
        instance = Instantiate(BasicShapes, GameObject.Find("Main Panel").transform);
        instance.name = "Basic Shapes"; // reset name
        GameObject.Find("Name Text").GetComponent<TextMeshProUGUI>().text = ""; // reset text
    }

    public void CustomProtein()
    {
        GameObject.Find("Asset Canvas").SetActive(true);
        hp.LoadAssetBundles(@"C:\Users\TEMP.CS-DOMAIN.001\Desktop\AssetBundles\AssetBundles\proteins");
    }
    public void instantiateProtein(Button btn)
    {
        Debug.Log(btn.name);
        Debug.Log(btn.transform.Find("text").gameObject.GetComponent<TextMeshProUGUI>().text);
        string proteinName = btn.transform.Find("text").gameObject.GetComponent<TextMeshProUGUI>().text;
        hp.InstantiateAsset(proteinName);
        GameObject.Find("Asset Canvas").SetActive(false);
    }

    public void GetProteinShape(string ShapeName)
    {
        GameObject instance;
        Vector3 SpawnPosition = new Vector3(1.859543f, 1.989825f, -2.135954f);
        Transform SpawnParent = GameObject.Find("Basic Shapes").transform;

        // instantiate more models
        if (ShapeName == "Sheet")
        {
            instance = Instantiate(Sheet, SpawnPosition, Quaternion.identity, SpawnParent);
            instance.name = "Sheet";
            
        } 
        else if (ShapeName == "Helix")
        {
            instance = Instantiate(Helix, SpawnPosition, Quaternion.identity, SpawnParent);
            instance.name = "Helix";
        }
        else if (ShapeName == "rCoil")
        {
            instance = Instantiate(rCoil, SpawnPosition, Quaternion.identity, SpawnParent);
            instance.name = "rCoil";
        }
        else if (ShapeName == "fCoil")
        {
            instance = Instantiate(fCoil, SpawnPosition, Quaternion.identity, SpawnParent);
            instance.name = "fCoil";
        } else
        {
            Debug.Log("Shape's name NOT EXIST!");
        }
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
