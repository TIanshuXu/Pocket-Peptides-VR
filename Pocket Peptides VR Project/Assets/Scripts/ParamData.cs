using UnityEngine;              // for basic purpose
using UnityEngine.Networking;   // for UnityWebRequest
using System.IO;                // for File
using System.Collections;       // for IEnumerator
using TMPro;                    // for TextMeshProUGUI

public class ParamData : MonoBehaviour
{
    string path;
    string jsonString;
    string computeURL;
    string downloadURL;
    string uploadURL;
    string nameOfModelOnDisplay;
    bool isProteinFieldExisting;

    GameObject  Texts;
    DropZone    dropZone;
    Proteins    proteins;
    //ManipulationIk mIK;

    float pI;
    float Mw;

    private void Start()
    {
        //mIK         = GameObject.Find("TempProtein").transform.Find("SheetSheetSheetSheet").GetComponent<ManipulationIk>();
        Texts       = this.transform.Find("Texts").gameObject;
        path        = Application.persistentDataPath + "/ProteinData.json";
        dropZone    = GameObject.Find("Protein Drop Panel").GetComponent<DropZone>();
        computeURL  = "https://web.expasy.org/cgi-bin/compute_pi/pi_tool";      // for hphob and pI
        downloadURL = "https://zhanglab.ccmb.med.umich.edu/I-TASSER/output/";   // for PDB files
        uploadURL   = "http://mordred.bioc.cam.ac.uk/~rapper/rampage2.php";     // for RC Score
        nameOfModelOnDisplay = ""; // set it empty first to initialize
        // fetch the Json Content
        if (File.Exists(path))
        {
            jsonString  = File.ReadAllText(path);
            proteins    = JsonUtility.FromJson<Proteins>("{\"proteins\":" + jsonString.ToString() + "}");
        }
        else
        { // TEMP
            // Debug.LogError("Json file not found in " + path);
        } // file not exist
    }
    private void Update()
    {
        // Change parameter if the Active Model changed
        foreach (Transform child in dropZone.ParentModel.transform)
        {
            if (child.gameObject.activeInHierarchy && nameOfModelOnDisplay != child.name)
            {
                nameOfModelOnDisplay = child.name;
                // OnParameterChange(); // change the parameters TEMP
            } // if active protein model changed
        }

        /*TEST*/
        if (Input.GetKeyDown("space"))
        {
            StartCoroutine(PostSequence());
        } // get pI and Mw values
        if (Input.GetKeyDown("d"))
        {
            StartCoroutine(DownloadPDB());
        } // download PDB file
        if (Input.GetKeyDown("u"))
        {
            StartCoroutine(UploadPDB());
        } // upload   PDB file
        if (Input.GetKeyDown("a"))
        {
            AppendJsonFile();
            Debug.Log("successfully appended");
        } // append   JSON file

        updateTargetBonesDistance();
    }

    void updateTargetBonesDistance()
    {
        if (dropZone.IsNameMatched)
        {
            Texts.transform.Find("DistanceText").gameObject.GetComponent<TextMeshProUGUI>().text = dropZone.targetPointsDistance.ToString();
        }
        else
        {
            Texts.transform.Find("DistanceText").gameObject.GetComponent<TextMeshProUGUI>().text = 0.ToString();
        }
    }
   
    IEnumerator UploadPDB()
    {
        string fileName = "model1.pdb";
        string filePath = Application.persistentDataPath + "/" + fileName;
        UnityWebRequest file = new UnityWebRequest();
        WWWForm         form = new WWWForm();
        file = UnityWebRequest.Get(filePath);
        yield return file.SendWebRequest();
        Debug.Log(file.downloadHandler.data); // debug works well, but the server is not working
        Debug.Log(file.downloadHandler.text);
        Debug.Log(Path.GetFileName(filePath));
        form.AddBinaryData("pdbfile", file.downloadHandler.data, Path.GetFileName(filePath));

        using (UnityWebRequest www = UnityWebRequest.Post(uploadURL, form))
        {
            yield return www.SendWebRequest();
            if(www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("success");
            }
        }
    }

    IEnumerator DownloadPDB()
    {
        string fileName = "model1.pdb";
        string jobID    = "S478362";
        string url      = downloadURL + jobID + "/" + fileName;
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string savePath = string.Format("{0}/{1}", Application.persistentDataPath, fileName);
                File.WriteAllText(savePath, www.downloadHandler.text);
                Debug.Log("PDB is saved to: " + savePath);
            }
        }
    }

    IEnumerator PostSequence()
    { 
        WWWForm form = new WWWForm(); // its field name has to be "protein"
        form.AddField("protein", "EAAAKEAAAKEAAAK"); // sequence is gonna be changed
        /* set up the POST request */
        using (UnityWebRequest www = UnityWebRequest.Post(computeURL, form))
        {
            yield return www.SendWebRequest();
            // check error or get parameters
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string allPageContent   = www.downloadHandler.text;
                string startPointToFind = "pI/Mw: ";
                string finalPointToFind = "<!-- sib_body -->";
                int    startIndex       = allPageContent.IndexOf(startPointToFind);
                int    finalIndex       = allPageContent.IndexOf(finalPointToFind);
                int    extractLength    = finalIndex - startIndex - startPointToFind.Length - 1;
                string allNumbers       = allPageContent.Substring(startIndex + startPointToFind.Length, extractLength);
                string[] numbers        = allNumbers.Split('/');
                pI = float.Parse(numbers[0]);
                Mw = float.Parse(numbers[1]);
                // print them out
                Debug.Log("pI = " + pI + ", Mw = " + Mw);
            }
        }
    }

    private void AppendJsonFile()
    {
        string filePath = Application.persistentDataPath + "/" + "ProteinData.json";
        string[] lines  = File.ReadAllLines(filePath);
        
        /* Test to write in a new json object */
        ProteinData pd = new ProteinData();
        pd.name = "haha";
        pd.sequence = "abc";
        string pdJson = JsonUtility.ToJson(pd);
        lines[lines.Length - 1] = "," + pdJson;
        System.Array.Resize(ref lines, lines.Length + 1);
        lines[lines.Length - 1] = "]";
        File.WriteAllLines(filePath, lines);
    }

    private void OnParameterChange()
    {
        foreach (ProteinData protein in proteins.proteins)
        {
            if (protein.name == nameOfModelOnDisplay && IsProteinFieldComplete(protein))
            {
                Texts.transform.Find("HphobText")       .gameObject.GetComponent<TextMeshProUGUI>().text = protein.hphob        .ToString();
                Texts.transform.Find("PIText")          .gameObject.GetComponent<TextMeshProUGUI>().text = protein.pI           .ToString();
                Texts.transform.Find("RPFavouredText")  .gameObject.GetComponent<TextMeshProUGUI>().text = protein.rPFavoured[0].ToString();
                Texts.transform.Find("RPAllowedText")   .gameObject.GetComponent<TextMeshProUGUI>().text = protein.rPAllowed[0] .ToString();
                Texts.transform.Find("CScoreText")      .gameObject.GetComponent<TextMeshProUGUI>().text = protein.cScore[0]    .ToString();
                isProteinFieldExisting = true;          // there is a Protein Field matched
            }
        }
        if (isProteinFieldExisting)
        {
            isProteinFieldExisting = false; // reset its value
        }
        else
        {
            // Debug.Log(nameOfModelOnDisplay + " is invalid in the Json File"); // bring this back later
        }  // the protein field doesn't exist or isn't complete
    }
    private bool IsProteinFieldComplete(ProteinData p)
    {
        return p.hphob != -1 && p.pI != -1 && p.rPFavoured != null && p.rPAllowed != null && p.cScore != null;
    }
}

[System.Serializable]
public class ProteinData
{
    public string  name;
    public string  sequence;
    public int     hphob = -1;
    public float   pI    = -1;
    public float[] rPFavoured;
    public float[] rPAllowed;
    public float[] cScore;
}

[System.Serializable]
public class Proteins
{
    public ProteinData[] proteins;
}